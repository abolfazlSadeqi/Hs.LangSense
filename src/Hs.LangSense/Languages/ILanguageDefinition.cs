namespace Hs.LangSense;

/// <summary>
/// Defines a language that Hs.LangSense can convert digits, letters, keyboard layouts and number-words for.
/// Implement this interface and call <see cref="LanguageRegistry.Register"/> to add a new language at runtime.
/// </summary>
public interface ILanguageDefinition
{
    /// <summary>Stable code used in ConvertTo, e.g. "fa", "tr", "de".</summary>
    string Code { get; }

    /// <summary>Maps to a <see cref="Language"/> value when it is a built-in language; otherwise use <see cref="Language.Unknown"/>.</summary>
    Language Language { get; }

    /// <summary>English display name.</summary>
    string DisplayName { get; }

    /// <summary>Digit characters for 0–9 (exactly 10 characters).</summary>
    string Digits { get; }

    /// <summary>Optional letter canonicalization map (variant → preferred form).</summary>
    IReadOnlyDictionary<char, char>? LetterMap { get; }

    /// <summary>Optional English-keyboard → this language layout map.</summary>
    IReadOnlyDictionary<char, char>? KeyboardFromEnglish { get; }

    /// <summary>Optional character → Latin transliteration segments.</summary>
    IReadOnlyDictionary<char, string>? TransliterationMap { get; }

    /// <summary>Convert a non-negative integer to words in this language. Return null to fall back to Persian.</summary>
    string? ToWords(long value);

    /// <summary>Try parse number-words in this language. Return false if not supported.</summary>
    bool TryParseWords(string? input, out decimal value);
}

/// <summary>
/// Registry of language definitions. Built-in languages are registered at startup.
/// Call <see cref="Register"/> to add Turkish, German, or any custom language without recompiling the library.
/// </summary>
/// <example>
/// <code>
/// LanguageRegistry.Register(new MyTurkishDefinition());
/// "123".ConvertDigitsUsing("tr");
/// NumberText.ToWords(42, "tr");
/// </code>
/// </example>
public static class LanguageRegistry
{
    private static readonly Dictionary<string, ILanguageDefinition> Map =
        new(StringComparer.OrdinalIgnoreCase);

    static LanguageRegistry()
    {
        // Built-ins are wired through existing engines; custom packs extend them.
        // Codes already handled by Text.ConvertTo remain the primary path for fa/ar/en/sorani/kurmanji.
    }

    /// <summary>Register or replace a language definition by its <see cref="ILanguageDefinition.Code"/>.</summary>
    public static void Register(ILanguageDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (string.IsNullOrWhiteSpace(definition.Code))
            throw new ArgumentException("Language code is required.", nameof(definition));
        if (definition.Digits is not { Length: 10 })
            throw new ArgumentException("Digits must contain exactly 10 characters (0–9).", nameof(definition));
        Map[definition.Code.Trim()] = definition;
    }

    /// <summary>Try get a registered custom language by code.</summary>
    public static bool TryGet(string code, out ILanguageDefinition? definition)
        => Map.TryGetValue(code, out definition);

    /// <summary>All registered custom language definitions.</summary>
    public static IReadOnlyCollection<ILanguageDefinition> All => Map.Values;

    /// <summary>Convert digits in text using a registered language's digit string.</summary>
    public static string ConvertDigits(string? text, string languageCode)
    {
        if (!TryGet(languageCode, out var def) || def is null)
            throw new KeyNotFoundException($"Language '{languageCode}' is not registered. Call LanguageRegistry.Register first.");
        var latin = DigitConverter.ToEnglish(text);
        var dig = def.Digits;
        var sb = new System.Text.StringBuilder(latin.Length);
        foreach (var c in latin)
            sb.Append(c is >= '0' and <= '9' ? dig[c - '0'] : c);
        return sb.ToString();
    }

    /// <summary>Number to words using a registered language, or built-in engines for fa/ar/en/sorani/kurmanji.</summary>
    public static string ToWords(long value, string languageCode)
    {
        if (TryGet(languageCode, out var def) && def is not null)
        {
            var w = def.ToWords(value);
            if (w is not null) return w;
        }
        return languageCode.Trim().ToLowerInvariant() switch
        {
            "fa" or "persian" or "farsi" => PersianWords.ToWords(value),
            "en" or "english" => EnglishNumberWords.ToWords(value),
            "sorani" or "ckb" => KurdishWords.ToSorani(value),
            "kurmanji" or "kmr" => KurdishWords.ToKurmanji(value),
            _ => throw new KeyNotFoundException($"No number-words engine for '{languageCode}'. Register an ILanguageDefinition.")
        };
    }

    /// <summary>Parse number-words using a registered language or built-in engines.</summary>
    public static bool TryParseWords(string? input, string languageCode, out decimal value)
    {
        if (TryGet(languageCode, out var def) && def is not null && def.TryParseWords(input, out value))
            return true;
        value = 0;
        return languageCode.Trim().ToLowerInvariant() switch
        {
            "fa" or "persian" or "farsi" => PersianWords.TryParse(input, out value),
            "en" or "english" => EnglishNumberWords.TryParse(input, out value),
            "sorani" or "ckb" => KurdishWords.TryParseSorani(input, out value),
            "kurmanji" or "kmr" => KurdishWords.TryParseKurmanji(input, out value),
            _ => false
        };
    }
}
