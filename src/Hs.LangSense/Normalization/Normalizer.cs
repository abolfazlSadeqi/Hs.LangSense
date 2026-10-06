using System.Globalization;
using System.Text;

namespace Hs.LangSense;

public sealed record NormalizationResult(string Value, IReadOnlyList<Transformation> Transformations);

/// <summary>
/// Canonicalizes multilingual text for Persian/Arabic/English processing.
/// Always prefer <see cref="Text.Normalize"/> or <c>str.NormalizeText()</c> —
/// not BCL <c>string.Normalize()</c> which only applies Unicode NFC/NFD.
/// </summary>
public static class Normalizer
{
    // Arabic letter forms (Unicode escapes — encoding-safe)
    private const char ArabicYeh = '\u064A';       // ي
    private const char ArabicAlefMaksura = '\u0649'; // ى
    private const char ArabicYehHamza = '\u0626';  // ئ
    private const char ArabicKaf = '\u0643';       // ك
    private const char ArabicTehMarbuta = '\u0629'; // ة
    private const char ArabicHehGoal = '\u06C0';   // ۀ
    private const char ArabicWawHamza = '\u0624';  // ؤ
    private const char ArabicAlefHamzaAbove = '\u0623'; // أ
    private const char ArabicAlefHamzaBelow = '\u0625'; // إ
    private const char ArabicAlefWasla = '\u0671'; // ٱ

    private const char PersianYeh = '\u06CC';      // ی
    private const char PersianKeheh = '\u06A9';    // ک
    private const char PersianHeh = '\u0647';      // ه
    private const char PersianWaw = '\u0648';      // و
    private const char PersianAlef = '\u0627';     // ا

    public static string Normalize(string? value, LangSenseOptions? options = null)
        => NormalizeDetailed(value, options).Value;

    public static NormalizationResult NormalizeDetailed(string? value, LangSenseOptions? options = null)
    {
        options ??= new LangSenseOptions();
        var s = value ?? "";
        var t = new List<Transformation>();

        void Apply(string name, TransformationKind kind, Func<string, string> f, bool safe = true)
        {
            var before = s;
            s = f(s);
            if (before != s) t.Add(new Transformation(kind, name, before, s, safe));
        }

        Apply("Unicode NFC", TransformationKind.Unicode, x => x.Normalize(NormalizationForm.FormC));

        if (options.ConvertPersianDigitsToLatin || options.ConvertArabicDigitsToLatin)
            Apply("Digits → Latin", TransformationKind.Digit, DigitConverter.ToEnglish);

        if (options.NormalizeArabicLetters)
            Apply("Arabic→Persian letters", TransformationKind.Letter, UnifyArabicPersianLetters);

        if (options.NormalizeTatweel)
            Apply("Tatweel", TransformationKind.Letter, x => x.Replace("\u0640", string.Empty)); // ـ

        if (options.RemoveBom)
            Apply("BOM", TransformationKind.Unicode, x => x.Replace("\uFEFF", string.Empty));

        Apply("Invisible marks", TransformationKind.Unicode, x => RemoveUnwantedMarks(x, options));

        Apply("Punctuation", TransformationKind.Punctuation, SymbolConverter.Normalize);

        if (options.RemoveDiacritics)
            Apply("Diacritics", TransformationKind.Unicode, RemoveMarks);

        if (options.NormalizeWhitespace)
            Apply("Whitespace", TransformationKind.Whitespace, NormalizeSpaces);

        if (options.NormalizeSigns)
            Apply("Signs", TransformationKind.Punctuation, x => x.Replace('\u2212', '-').Replace('\u2013', '-').Replace('\u2014', '-'));

        if (options.RemoveControlCharacters)
            Apply("Controls", TransformationKind.Unicode,
                x => new string(x.Where(c => !char.IsControl(c) || c is '\n' or '\r' or '\t').ToArray()));

        if (options.Trim)
            Apply("Trim", TransformationKind.Whitespace, x => x.Trim());

        return new NormalizationResult(s, t);
    }

    public static string Persian(string? s) => NormalizePersian(s);

    public static string NormalizePersian(string? s)
        => Normalize(s, new LangSenseOptions
        {
            ConvertPersianDigitsToLatin = false,
            ConvertArabicDigitsToLatin = false,
            NormalizeArabicLetters = true,
            PreserveZeroWidthNonJoiner = false,
            Trim = true,
            NormalizeWhitespace = true
        });

    /// <summary>
    /// Canonical Arabic presentation forms → Persian preferred forms:
    /// ي/ى → ی ، ك → ک ، ة/ۀ → ه ، أ/إ/ٱ → ا ، ؤ → و
    /// </summary>
    public static string UnifyArabicPersianLetters(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            sb.Append(c switch
            {
                ArabicYeh or ArabicAlefMaksura => PersianYeh,
                ArabicKaf => PersianKeheh,
                ArabicHehGoal or ArabicTehMarbuta => PersianHeh,
                ArabicWawHamza => PersianWaw,
                ArabicAlefHamzaAbove or ArabicAlefHamzaBelow or ArabicAlefWasla => PersianAlef,
                _ => c
            });
        }
        return sb.ToString();
    }

    private static string NormalizeSpaces(string s)
    {
        s = s.Replace('\u00A0', ' ').Replace('\u2009', ' ').Replace('\u202F', ' ');
        return string.Join(' ', s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string RemoveMarks(string s)
        => new string(s.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

    private static string RemoveUnwantedMarks(string s, LangSenseOptions options)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (c == '\u200C' && !options.PreserveZeroWidthNonJoiner) continue;
            if (c == '\u200D' && !options.PreserveZeroWidthJoiner) continue;
            if (c is '\u200E' or '\u200F' or '\u200B' or '\u2060' or '\u00AD') continue;
            sb.Append(c);
        }
        return sb.ToString();
    }
}
