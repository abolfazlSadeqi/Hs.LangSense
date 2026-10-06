namespace Hs.LangSense;

/// <summary>
/// Legacy simple language map holder. Prefer <see cref="ILanguageDefinition"/> and <see cref="LanguageRegistry"/>.
/// </summary>
public sealed class LanguageDefinition
{
    public string Code { get; init; } = "";
    public string Name { get; init; } = "";
    public IReadOnlyDictionary<char, char> CharacterMap { get; init; } = new Dictionary<char, char>();
    public IReadOnlyDictionary<string, string> TransliterationMap { get; init; } = new Dictionary<string, string>();
}

/// <summary>Legacy converter. Prefer <see cref="Text.ConvertTo"/>.</summary>
[Obsolete("Use Text.ConvertTo instead.")]
public static class LanguageConverter
{
    public static string Convert(string value, string from, string to)
    {
        if (from.Equals(to, StringComparison.OrdinalIgnoreCase)) return value;
        if (from.Equals("fa", StringComparison.OrdinalIgnoreCase) && to.Equals("en", StringComparison.OrdinalIgnoreCase))
            return DigitConverter.ToEnglish(value);
        if (from.Equals("ar", StringComparison.OrdinalIgnoreCase) && to.Equals("fa", StringComparison.OrdinalIgnoreCase))
            return Normalizer.Normalize(value, new LangSenseOptions { ConvertPersianDigitsToLatin = false, ConvertArabicDigitsToLatin = false });
        return value;
    }
}
