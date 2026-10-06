namespace Hs.LangSense;

/// <summary>
/// Example of how to add a new language. Copy this class, change the maps, and call
/// <c>LanguageRegistry.Register(new TurkishLanguageDefinition());</c> at startup.
/// This pack is NOT registered automatically — it is a template for consumers.
/// </summary>
public sealed class TurkishLanguageDefinition : ILanguageDefinition
{
    public string Code => "tr";
    public Language Language => Language.Unknown;
    public string DisplayName => "Turkish";
    public string Digits => "0123456789"; // Turkish uses Latin digits

    public IReadOnlyDictionary<char, char>? LetterMap => null;
    public IReadOnlyDictionary<char, char>? KeyboardFromEnglish => null;
    public IReadOnlyDictionary<char, string>? TransliterationMap => null;

    public string? ToWords(long value)
    {
        // Minimal demo — replace with full Turkish number words in production
        if (value == 0) return "sıfır";
        if (value < 0) return "eksi " + ToWords(-value);
        // Fall through to a tiny subset for the sample
        return value switch
        {
            1 => "bir", 2 => "iki", 3 => "üç", 4 => "dört", 5 => "beş",
            10 => "on", 20 => "yirmi", 100 => "yüz", 1000 => "bin",
            _ => value.ToString() // incomplete demo
        };
    }

    public bool TryParseWords(string? input, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var map = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
        {
            ["sıfır"] = 0, ["bir"] = 1, ["iki"] = 2, ["üç"] = 3, ["dört"] = 4, ["beş"] = 5,
            ["on"] = 10, ["yirmi"] = 20, ["yüz"] = 100, ["bin"] = 1000
        };
        if (!map.TryGetValue(input.Trim(), out var n)) return false;
        value = n;
        return true;
    }
}
