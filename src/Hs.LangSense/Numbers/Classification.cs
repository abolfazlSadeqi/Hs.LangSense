namespace Hs.LangSense;
public sealed record NumberClassification(ValueType Type, decimal? Value, bool IsValid, string Reason);
public static class NumberIntelligence
{
    public static NumberClassification Classify(string input, ValueContext? context = null)
    {
        if (context.HasValue)
        {
            NumberClassification r = context.Value switch
            {
                ValueContext.Percentage => PercentageParser.TryParse(input, out var p) ?
                new(ValueType.Percentage, p, p >= 0 && p <= 100, "Context percentage") :
                new(ValueType.Percentage, null, false, "Invalid percentage"),
                ValueContext.Score => decimal.TryParse(Normalizer.Normalize(input), out var s) ?
                new(ValueType.Score, s, s >= 0 && s <= 20, "Context score") :
                new(ValueType.Score, null, false, "Invalid score"),
                ValueContext.Money => Money.TryParse(input, out var m) ? new(ValueType.Money, m.Amount, true, "Context money") : new(ValueType.Money, null, false, "Invalid money"),
                _ => new(ValueType.Unknown, null, false, "Unknown context")
            }; return r;
        }
        var d = Detector.Detect(input); return NumberParser.TryParse(input, out var n) ? new(d.Type, n, d.IsValid, "Auto detection") : new(d.Type, null, d.IsValid, "Detected non-numeric value");
    }
}
