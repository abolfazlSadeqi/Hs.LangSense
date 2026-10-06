using System.Net;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public static class TextExtractor
{
    private static readonly Regex NumberRegex = new(@"(?<![\p{L}\d])[-+]?\d+(?:[.,]\d+)?(?![\p{L}\d])", RegexOptions.Compiled);
    private static readonly Regex PersianNumberRegex = new(@"(?<![\p{L}\d])[-+]?([۰-۹٠-٩]+)(?:[.,٬٫]([۰-۹٠-٩]+))?(?![\p{L}\d])", RegexOptions.Compiled);

    public static IReadOnlyList<TextOccurrence> Extract(string? text)
    {
        var source = text ?? string.Empty;
        var all = new List<TextOccurrence>();
        Add(all, source, Patterns.Email);
        Add(all, source, Patterns.Url);
        Add(all, source, Patterns.IBAN);
        Add(all, source, Patterns.BankCard);
        Add(all, source, Patterns.Mobile);
        Add(all, source, PatternLibrary.PersianMobile);
        Add(all, source, Patterns.NationalCode);
        Add(all, source, Patterns.LegalNationalCode);
        Add(all, source, Patterns.PostalCode);
        Add(all, source, Patterns.IPv4);
        Add(all, source, Patterns.IPv6);
        Add(all, source, Patterns.Date);
        Add(all, source, PatternLibrary.PersianDate);
        Add(all, source, Patterns.Percentage);
        Add(all, source, Patterns.Money);
        Add(all, source, Patterns.Sayad);
        Add(all, source, Patterns.ChequeNumber);
        Add(all, source, Patterns.AccountNumber);

        foreach (Match m in NumberRegex.Matches(source)) AddNumber(all, source, m);
        foreach (Match m in PersianNumberRegex.Matches(source)) AddNumber(all, source, m);

        return ResolveOverlaps(all);
    }

    public static IReadOnlyList<TextOccurrence> Extract(ValueType type, string text)
        => Extract(text).Where(x => x.Type == type).ToList();

    public static IReadOnlyList<TextOccurrence> Extract(string text, ValueType type) => Extract(type, text);

    public static IEnumerable<string> ExtractEmails(string text) => Extract(text, ValueType.Email).Select(x => x.Value);
    public static IEnumerable<string> ExtractMobiles(string text) => Extract(text, ValueType.IranianMobile).Select(x => x.Value);
    public static IEnumerable<string> ExtractCards(string text) => Extract(text, ValueType.BankCard).Select(x => x.Value);
    public static IEnumerable<string> ExtractIBANs(string text) => Extract(text, ValueType.IBAN).Select(x => x.Value);
    public static IEnumerable<string> ExtractUrls(string text) => Extract(text, ValueType.Url).Select(x => x.Value);
    public static IEnumerable<string> ExtractDates(string text) => Extract(text).Where(x => x.Type is ValueType.Date or ValueType.PersianDate).Select(x => x.Value);
    public static IEnumerable<string> ExtractSayad(string text) => Extract(text, ValueType.Sayad).Select(x => x.Value);
    public static IEnumerable<string> ExtractNumbers(string text) => Extract(text).Where(x => x.Type == ValueType.Number).Select(x => x.Value);

    public static IReadOnlyList<TextOccurrence> ExtractWithContext(string text, ValueContext context)
    {
        var result = new List<TextOccurrence>();
        foreach (var item in Extract(text))
        {
            var contextual = new ContextualValue(item.Value, context).Analyze();
            if (contextual.Success)
                result.Add(item with { Type = contextual.Type, Normalized = contextual.NormalizedValue ?? item.Normalized, IsValid = true, Confidence = Math.Max(item.Confidence, contextual.Confidence) });
        }
        return result;
    }

    private static void Add(List<TextOccurrence> target, string source, ValuePattern pattern)
    {
        foreach (var match in pattern.Extract(source))
        {
            var normalized = Normalizer.Normalize(match.Value);
            target.Add(new TextOccurrence(match.Value, normalized, match.Type, match.Start, match.Length,
                match.IsValid ? .99 : .45, match.IsValid, match.Issues));
        }
    }

    private static void AddNumber(List<TextOccurrence> target, string source, Match match)
    {
        if (match.Length == 0) return;
        var raw = match.Value;
        if (target.Any(x => x.Start == match.Index && x.Length == match.Length)) return;
        if (IranianMobile.IsValid(raw) || BankCard.IsValid(raw) || IranianNationalCode.IsValid(raw) || IranianPostalCode.IsValid(raw)) return;
        target.Add(new TextOccurrence(raw, Normalizer.Normalize(raw), ValueType.Number, match.Index, match.Length, .9,
            NumberParser.TryParse(raw, out _), Array.Empty<ValidationIssue>()));
    }

    private static IReadOnlyList<TextOccurrence> ResolveOverlaps(IEnumerable<TextOccurrence> occurrences)
    {
        var priority = new Dictionary<ValueType, int>
        {
            [ValueType.IBAN] = 100,
            [ValueType.BankCard] = 95,
            [ValueType.IranianMobile] = 90,
            [ValueType.Email] = 88,
            [ValueType.NationalCode] = 80,
            [ValueType.LegalNationalCode] = 78,
            [ValueType.PostalCode] = 76,
            [ValueType.Url] = 74,
            [ValueType.IPv6] = 72,
            [ValueType.IPv4] = 70,
            [ValueType.Money] = 68,
            [ValueType.Percentage] = 66,
            [ValueType.Date] = 64,
            [ValueType.PersianDate] = 64,
            [ValueType.Sayad] = 60,
            [ValueType.ChequeNumber] = 55,
            [ValueType.AccountNumber] = 50,
            [ValueType.Number] = 10
        };
        var selected = new List<TextOccurrence>();
        foreach (var item in occurrences
            .OrderByDescending(x => priority.TryGetValue(x.Type, out var p) ? p : 0)
            .ThenByDescending(x => x.Length).ThenBy(x => x.Start))
        {
            if (selected.Any(x => x.Start < item.Start + item.Length && item.Start < x.Start + x.Length)) continue;
            selected.Add(item);
        }
        return selected.OrderBy(x => x.Start).ThenByDescending(x => x.Length).ToList();
    }
}
