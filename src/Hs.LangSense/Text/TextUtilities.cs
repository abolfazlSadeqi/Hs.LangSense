using System.Text;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

[Flags]
public enum PersianNormalizerFlags
{
    None = 0, ApplyPersianCharacters = 1, ApplyHalfSpaceRule = 2,
    CleanupSpacingAndLineBreaks = 4, RemoveDiacritics = 8, CleanupPunctuation = 16,
    NormalizeDigits = 32, All = 63
}

public static class Textify
{
    public static string Number(long value, Language language = Language.Persian) => language switch
    {
        Language.English => EnglishNumberWords.ToWords(value),
        _ => PersianWords.ToWords(value)
    };
}

public static class PersianTextUtilities
{
    public static bool ContainsOnlyPersianLetters(string? value) => !string.IsNullOrEmpty(value) && value.All(c => PersianText.IsPersianLetter(c) || char.IsWhiteSpace(c));
    public static bool ContainsOnlyPersianOrEnglishLetters(string? value) => !string.IsNullOrEmpty(value) && value.All(c => PersianText.IsPersianLetter(c) || (c is >= 'A' and <= 'Z') || (c is >= 'a' and <= 'z') || char.IsWhiteSpace(c));
    public static string NormalizePersianText(string? value, PersianNormalizerFlags flags = PersianNormalizerFlags.All)
    {
        var s = value ?? string.Empty;
        if (flags.HasFlag(PersianNormalizerFlags.ApplyPersianCharacters)) s = s.Replace('ي', 'ی').Replace('ى', 'ی').Replace('ك', 'ک').Replace('ۀ', 'ه').Replace('ة', 'ه').Replace('ؤ', 'و').Replace('إ', 'ا').Replace('أ', 'ا').Replace('ٱ', 'ا');
        if (flags.HasFlag(PersianNormalizerFlags.NormalizeDigits)) s = DigitConverter.ToEnglish(s);
        if (flags.HasFlag(PersianNormalizerFlags.RemoveDiacritics)) s = Regex.Replace(s, "[\\u064B-\\u065F\\u0670]", "");
        if (flags.HasFlag(PersianNormalizerFlags.ApplyHalfSpaceRule)) s = Regex.Replace(s, "\\s*\\u200C\\s*", "\\u200C");
        if (flags.HasFlag(PersianNormalizerFlags.CleanupSpacingAndLineBreaks)) s = Regex.Replace(s, "[ \\t\\r\\n]+", " ").Trim();
        if (flags.HasFlag(PersianNormalizerFlags.CleanupPunctuation)) s = s.Replace('،', ',').Replace('؛', ';').Replace('؟', '?');
        return s;
    }
    public static string RemoveInvisibleCharacters(string? value) => new string((value ?? string.Empty).Where(c => !char.GetUnicodeCategory(c).ToString().Contains("Format", StringComparison.OrdinalIgnoreCase) && c != '\uFEFF').ToArray());
    public static string CollapseWhitespace(string? value) => Regex.Replace(value ?? string.Empty, "\\s+", " ").Trim();
    public static string ToSingleLine(string? value) => Regex.Replace(value ?? string.Empty, "[\\r\\n]+", " ");
}
