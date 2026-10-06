using System.Text;

namespace Hs.LangSense;

public sealed record OcrCleanResult(string Original, string Cleaned, double Confidence, IReadOnlyList<RepairSuggestion> Suggestions);

public static class RepairEngine
{
    private static readonly Dictionary<char, char> CommonOcr = new()
    {
        ['٠'] = '0',
        ['١'] = '1',
        ['٢'] = '2',
        ['٣'] = '3',
        ['٤'] = '4',
        ['٥'] = '5',
        ['٦'] = '6',
        ['٧'] = '7',
        ['٨'] = '8',
        ['٩'] = '9',
        ['۰'] = '0',
        ['۱'] = '1',
        ['۲'] = '2',
        ['۳'] = '3',
        ['۴'] = '4',
        ['۵'] = '5',
        ['۶'] = '6',
        ['۷'] = '7',
        ['۸'] = '8',
        ['۹'] = '9'
    };
    public static OcrCleanResult CleanOcr(string? text)
    {
        var original = text ?? string.Empty; var current = original; var suggestions = new List<RepairSuggestion>();
        var normalized = Normalizer.NormalizeDetailed(current, new LangSenseOptions { ConvertPersianDigitsToLatin = true, ConvertArabicDigitsToLatin = true, NormalizeWhitespace = true, Trim = true });
        current = normalized.Value;
        foreach (var kv in CommonOcr)
        {
            if (!current.Contains(kv.Key)) continue;
            var before = current; current = current.Replace(kv.Key, kv.Value); suggestions.Add(new(before, current, true, $"OCR digit normalization: {kv.Key}→{kv.Value}", .95));
        }
        current = CollapseRepeatedPunctuation(current, suggestions);
        var confidence = original.Length == 0 ? 1d : Math.Max(.5d, 1d - (double)suggestions.Count / Math.Max(1, original.Length));
        return new(original, current, confidence, suggestions);
    }
    private static string CollapseRepeatedPunctuation(string value, List<RepairSuggestion> suggestions)
    {
        var sb = new StringBuilder(); char last = '\0';
        foreach (var c in value)
        {
            if ((c == '.' || c == ',' || c == '-') && c == last) continue;
            sb.Append(c); last = c;
        }
        var result = sb.ToString();
        if (result != value) suggestions.Add(new(value, result, true, "Repeated punctuation collapsed", .9));
        return result;
    }
    public static IReadOnlyList<RepairSuggestion> Suggest(string? value)
    {
        var original = value ?? string.Empty; var result = CleanOcr(original); return result.Suggestions;
    }
}

public sealed class DefaultOcrCleaner : IOcrCleaner
{
    public string Clean(string text, out IReadOnlyList<RepairSuggestion> suggestions)
    {
        var result = RepairEngine.CleanOcr(text); suggestions = result.Suggestions; return result.Cleaned;
    }
}
