namespace Hs.LangSense.Batch;

public sealed record BatchItemResult(int Index, string Raw, ValueResult<string> Result);
public sealed record BatchResult(IReadOnlyList<BatchItemResult> Items)
{
    public int Count => Items.Count;
    public int ValidCount => Items.Count(x => x.Result.IsValid);
    public int InvalidCount => Count - ValidCount;
    public double SuccessRate => Count == 0 ? 1 : ValidCount / (double)Count;
}

/// <summary>Prefer <see cref="SmartValue.AnalyzeMany"/> for batch analysis.</summary>
[System.Obsolete("Use SmartValue.AnalyzeMany for batch value analysis.")]
public static class BatchProcessor
{
    public static BatchResult Analyze(IEnumerable<string?> values, ValueContext? context = null)
    {
        var list = values?.ToList() ?? throw new ArgumentNullException(nameof(values));
        var items = new List<BatchItemResult>(list.Count);
        for (var i = 0; i < list.Count; i++)
        {
            var raw = list[i] ?? string.Empty;
            var result = context is null ? LangSense.Analyze(raw) : new ContextualValue(raw, context.Value).Analyze();
            items.Add(new BatchItemResult(i, raw, result));
        }
        return new BatchResult(items);
    }

    public static IEnumerable<BatchItemResult> Stream(IEnumerable<string?> values, ValueContext? context = null)
    {
        var i = 0;
        foreach (var raw in values ?? throw new ArgumentNullException(nameof(values)))
        {
            var value = raw ?? string.Empty;
            yield return new BatchItemResult(i++, value, context is null ? LangSense.Analyze(value) : new ContextualValue(value, context.Value).Analyze());
        }
    }
}
