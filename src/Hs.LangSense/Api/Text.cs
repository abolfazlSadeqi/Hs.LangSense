using System.Globalization;

namespace Hs.LangSense;

/// <summary>Provides the small, high-level API for the most common text and value operations.</summary>
public static class Text
{
    /// <summary>
    /// Normalizes text: Arabic→Persian letters (ي→ی، ك→ک), optional digit conversion to Latin,
    /// removes ZWNJ/invisible marks, collapses whitespace, trims.
    /// </summary>
    public static string Normalize(string? value, NormalizationProfile? profile = null)
    {
        if (profile is not null)
            return Normalizer.Normalize(value, profile.Options);
        // Explicit defaults so callers always get letter unify + clean whitespace
        return Normalizer.Normalize(value, new LangSenseOptions
        {
            ConvertPersianDigitsToLatin = true,
            ConvertArabicDigitsToLatin = true,
            NormalizeArabicLetters = true,
            NormalizeWhitespace = true,
            PreserveZeroWidthNonJoiner = false,
            PreserveZeroWidthJoiner = false,
            Trim = true,
            NormalizeTatweel = true,
            NormalizeSigns = true
        });
    }

    /// <summary>Converts a string's script, digits and deterministic language representation to the requested language.</summary>
    /// <remarks>This is deterministic conversion, not machine translation. Use <see cref="Translate"/> for provider-backed translation.</remarks>
    public static string Convert(string? value, Language target, Language? source = null, TextConversionOptions? options = null)
    {
        var input = value ?? string.Empty;
        var detected = source ?? LanguageDetector.Detect(input);
        var result = input;

        if (options?.NormalizeFirst != false)
            result = Normalizer.Normalize(result, new LangSenseOptions { ConvertPersianDigitsToLatin = false, ConvertArabicDigitsToLatin = false });

        if (target == Language.Persian)
        {
            result = Normalizer.NormalizePersian(result);
            if (options?.ConvertDigits ?? true) result = DigitConverter.ToPersian(result);
        }
        else if (target == Language.Arabic)
        {
            result = result.Replace('ی', 'ي').Replace('ک', 'ك'); // keep گ as-is (not Arabic letter)
            if (options?.ConvertDigits ?? true) result = DigitConverter.ToArabicIndic(result);
        }
        else if (target == Language.Sorani)
        {
            result = Normalizer.NormalizePersian(result);
            // Sorani uses Arabic script; keep Kurdish letters ێ ە ڵ ڕ ڤ ۆ
            if (options?.ConvertDigits ?? true) result = DigitConverter.ToPersian(result);
        }
        else if (target == Language.Kurmanji)
        {
            result = Normalizer.NormalizePersian(result);
            if (detected is Language.Persian or Language.Arabic or Language.Sorani or Language.Mixed)
                result = Transliterator.ToLatin(result);
            if (options?.ConvertDigits ?? true) result = DigitConverter.ToEnglish(result);
        }
        else if (target == Language.English || target == Language.Latin)
        {
            result = Normalizer.NormalizePersian(result);
            if (detected is Language.Persian or Language.Arabic or Language.Sorani or Language.Mixed)
                result = Transliterator.ToLatin(result);
            if (options?.ConvertDigits ?? true) result = DigitConverter.ToEnglish(result);
        }

        if (options?.NormalizeAfter != false)
            result = Normalizer.Normalize(result, new LangSenseOptions { ConvertPersianDigitsToLatin = false, ConvertArabicDigitsToLatin = false });

        return result;
    }

    /// <summary>Converts a string to a target language using the complete deterministic conversion pipeline.</summary>
    public static string ConvertTo(string? value, Language target, TextConversionOptions? options = null)
        => Convert(value, target, null, options);

    /// <summary>Converts a string to a target language code such as <c>fa</c>, <c>ar</c> or <c>en</c>.</summary>
    public static string ConvertTo(string? value, string language, TextConversionOptions? options = null)
    {
        var target = language.Trim().ToLowerInvariant() switch
        {
            "fa" or "fa-ir" or "persian" or "farsi" => Language.Persian,
            "ar" or "ar-sa" or "arabic" => Language.Arabic,
            "en" or "en-us" or "en-gb" or "english" or "latin" => Language.English,
            "sorani" or "ckb" or "ku" or "ku-arab" => Language.Sorani,
            "kurmanji" or "kmr" or "ku-latn" => Language.Kurmanji,
            _ => throw new ArgumentException("Supported languages: fa, ar, en, sorani, kurmanji.", nameof(language))
        };
        return Convert(value, target, null, options);
    }

    /// <summary>Converts a string to Persian using the library's deterministic Persian conversion pipeline.</summary>
    public static string ToPersian(string? value, TextConversionOptions? options = null) => Convert(value, Language.Persian, null, options);

    /// <summary>Converts a string to Arabic using the library's deterministic Arabic conversion pipeline.</summary>
    public static string ToArabic(string? value, TextConversionOptions? options = null) => Convert(value, Language.Arabic, null, options);

    /// <summary>Latin transliteration + Latin digits (deterministic). Not semantic translation. Use <see cref="Transliterate"/> for script-only conversion.</summary>
    public static string ToEnglish(string? value, TextConversionOptions? options = null) => Convert(value, Language.English, null, options);

    /// <summary>Applies the complete deterministic preparation pipeline: Unicode cleanup, script normalization, digit normalization and target conversion.</summary>
    public static string Prepare(string? value, Language target = Language.Persian, TextConversionOptions? options = null)
        => Convert(value, target, null, options);

    /// <summary>Converts only the numeric system while preserving all other characters.</summary>
    public static string Digits(string? value, NumberSystem target)
        => DigitConverter.FromSystem(value, target);

    /// <summary>Converts a keyboard-layout mistake such as <c>jsj</c> to its intended Persian or Arabic text.</summary>
    public static string Keyboard(string? value, KeyboardLayout from, KeyboardLayout to, KeyboardLayoutDefinition? custom = null)
        => KeyboardLayouts.Convert(value, from, to, custom);

    /// <summary>Transliterates Persian or Arabic script into a Latin phonetic representation.</summary>
    public static string Transliterate(string? value) => Transliterator.ToLatin(value);

    /// <summary>Analyzes a value and returns its detected type, normalized representation, confidence and validation details.</summary>
    public static ValueResult<string> Analyze(object? value, ValueContext? context = null, LangSenseOptions? options = null)
        => LangSense.Analyze(value, options, context);

    /// <summary>Validates a value using one or more composable LangSense rules.</summary>
    public static ValueResult<string> Validate(string? value, params IRule[] rules)
        => LangSense.Validate(value ?? string.Empty, rules);

    /// <summary>Extracts all supported values from free text, including position, normalization, confidence and validity.</summary>
    public static IReadOnlyList<TextOccurrence> Extract(string? value) => TextExtractor.Extract(value ?? string.Empty);

    /// <summary>Compares two strings using semantic normalization appropriate for multilingual search and equality.</summary>
    public static bool Same(string? left, string? right, ComparisonMode mode = ComparisonMode.Semantic)
        => ValueComparer.Equals(left, right, mode);

    /// <summary>Returns a stable semantic fingerprint suitable for deduplication and search indexing.</summary>
    public static string Fingerprint(string? value, ComparisonMode mode = ComparisonMode.Semantic)
        => SemanticFingerprint.Create(value, mode);

    /// <summary>Masks detected sensitive values such as cards, IBANs, mobile numbers and email addresses.</summary>
    public static string Mask(string? value) => SensitiveData.Mask(value ?? string.Empty);

    /// <summary>Redacts detected sensitive values from a string.</summary>
    public static string Redact(string? value) => SensitiveData.Redact(value ?? string.Empty);

    /// <summary>Parses a natural Persian date expression such as امروز, فردا, پس‌فردا or اول مهر.</summary>
    public static DateTime? Date(string? value, DateTime? now = null) => DateParser.Relative(value ?? string.Empty, now);

    /// <summary>Parses a universal numeric value from Persian, Arabic or English digits and number words.</summary>
    public static bool Number(string? value, out decimal result) => NumberParser.TryParse(value, out result);

    /// <summary>Parses a money value including Persian currency words and K/M/B-style scales.</summary>
    public static bool Money(string? value, out MoneyValue result) => Hs.LangSense.Money.TryParse(value, out result);

    /// <summary>Scrubs sensitive information before a string is written to logs.</summary>
    public static string ScrubLog(string? value) => LogScrubber.Scrub(value ?? string.Empty);

    /// <summary>Convert a number to words in a language code (fa, en, sorani, kurmanji, or a registered custom code).</summary>
    public static string NumberToWords(long value, string languageCode)
        => LanguageRegistry.ToWords(value, languageCode);

    /// <summary>Convert a number to words using the <see cref="Language"/> enum.</summary>
    public static string NumberToWords(long value, Language language) => language switch
    {
        Language.English or Language.Latin => EnglishNumberWords.ToWords(value),
        Language.Sorani => KurdishWords.ToSorani(value),
        Language.Kurmanji => KurdishWords.ToKurmanji(value),
        _ => PersianWords.ToWords(value)
    };

    /// <summary>Parse number-words in a specific language code into a decimal.</summary>
    public static bool WordsToNumber(string? words, string languageCode, out decimal value)
        => LanguageRegistry.TryParseWords(words, languageCode, out value);

    /// <summary>Parse number-words (tries Persian, Sorani, Kurmanji) into a decimal.</summary>
    public static bool WordsToNumber(string? words, out decimal value)
    {
        if (PersianWords.TryParse(words, out value)) return true;
        if (EnglishNumberWords.TryParse(words, out value)) return true;
        if (KurdishWords.TryParseSorani(words, out value)) return true;
        if (KurdishWords.TryParseKurmanji(words, out value)) return true;
        return false;
    }

    /// <summary>Translate number-words from any built-in language into words of the target language code.</summary>
    public static string TranslateNumberWords(string? words, string targetLanguageCode)
    {
        if (!WordsToNumber(words, out var n)) return words ?? "";
        return NumberToWords((long)n, targetLanguageCode);
    }

    /// <summary>Runs a provider-backed machine translation when a translator has been configured.</summary>
    public static string Translate(string? value, string targetLanguage, string? sourceLanguage = null, ILanguageTranslator? translator = null)
    {
        if (translator is null) throw new InvalidOperationException("No language translator is configured. Use Text.Convert for deterministic conversion or register an ILanguageTranslator for semantic translation.");
        return translator.Translate(value ?? string.Empty, sourceLanguage, targetLanguage);
    }
}

/// <summary>Controls the behavior of deterministic text conversion.</summary>
public sealed record TextConversionOptions
{
    /// <summary>Normalizes the input before conversion.</summary>
    public bool NormalizeFirst { get; init; } = true;
    /// <summary>Normalizes the output after conversion.</summary>
    public bool NormalizeAfter { get; init; } = false;
    /// <summary>Converts digits to the target language's common digit system.</summary>
    public bool ConvertDigits { get; init; } = true;
}

/// <summary>Represents an optional semantic translation provider.</summary>
public interface ILanguageTranslator
{
    /// <summary>Translates text from an optional source language to the target language.</summary>
    string Translate(string text, string? sourceLanguage, string targetLanguage);
}

/// <summary>Detects the dominant script/language family of text without requiring an external translation service.</summary>
public static class LanguageDetector
{
    /// <summary>Detects Persian, Arabic, English, numeric or mixed script.</summary>
    public static Language Detect(string? value)
    {
        var text = value ?? string.Empty;
        var fa = text.Count(c => c is >= '\u0600' and <= '\u06FF' && PersianText.IsPersianLetter(c));
        var ar = text.Count(c => c is >= '\u0600' and <= '\u06FF' && !PersianText.IsPersianLetter(c) && char.IsLetter(c));
        var en = text.Count(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z');
        var numeric = text.Count(char.IsDigit);
        var total = fa + ar + en + numeric;
        if (total == 0) return Language.Unknown;
        var kinds = new[] { (fa, Language.Persian), (ar, Language.Arabic), (en, Language.English), (numeric, Language.Numeric) }.Where(x => x.Item1 > 0).Select(x => x.Item2).Distinct().ToList();
        if (kinds.Count == 1) return kinds[0];
        return Language.Mixed;
    }
}
