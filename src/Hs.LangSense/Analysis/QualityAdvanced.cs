namespace Hs.LangSense;

public sealed record ColumnQuality(
    string Name,
    int Total,
    int Empty,
    int Valid,
    int Invalid,
    double Completeness,
    double Validity,
    ValueType DominantType);

public sealed record DatasetQualityReport(
    int RowCount,
    IReadOnlyList<ColumnQuality> Columns,
    IReadOnlyList<DuplicateGroup<string>> DuplicateGroups,
    double OverallScore);

public static class DatasetQuality
{
    public static DatasetQualityReport Analyze(IEnumerable<IDictionary<string,object?>> rows, params string[] keyColumns)
    {
        var materialized = rows.ToList();
        var profiles = DatasetAnalyzer.Analyze(materialized).Profiles;
        var columns = profiles.Select(profile =>
        {
            var values = materialized.Select(r => r.TryGetValue(profile.Name, out var v) ? v : null).ToList();
            var nonEmpty = values.Where(v => !string.IsNullOrWhiteSpace(v?.ToString())).ToList();
            var valid = nonEmpty.Count(v => SmartValue.Analyze(v).IsValid);
            return new ColumnQuality(profile.Name, values.Count, values.Count - nonEmpty.Count, valid, nonEmpty.Count - valid,
                profile.Completeness, nonEmpty.Count == 0 ? 1 : (double)valid / nonEmpty.Count, profile.DominantType);
        }).ToList();

        var duplicates = keyColumns.Length == 0
            ? Array.Empty<DuplicateGroup<string>>()
            : DuplicateDetector.Find(materialized, row => string.Join("|", keyColumns.Select(k => row.TryGetValue(k, out var v) ? v?.ToString() : string.Empty)));
        var score = columns.Count == 0 ? 1 : columns.Average(x => x.Completeness * .4 + x.Validity * .6);
        return new(materialized.Count, columns, duplicates, score);
    }
}
