using System.Collections;

namespace Hs.LangSense;

public sealed record DataQualityIssue(string Code, string Message, int Index, object? Value, ValueType DetectedType);

public sealed record DataQualityReport(
    int TotalCount,
    int ValidCount,
    int InvalidCount,
    int EmptyCount,
    double Completeness,
    double Validity,
    IReadOnlyDictionary<ValueType,int> TypeCounts,
    IReadOnlyList<DataQualityIssue> Issues)
{
    public double QualityScore => TotalCount == 0 ? 1d : Math.Max(0d, (Completeness * .4d) + (Validity * .6d));
    public override string ToString() => $"Total={TotalCount}, Valid={ValidCount}, Invalid={InvalidCount}, Empty={EmptyCount}, Completeness={Completeness:P0}, Validity={Validity:P0}, Score={QualityScore:P0}";
}

public static class DataQuality
{
    public static DataQualityReport Analyze(IEnumerable<object?> values)
    {
        var list = values?.ToList() ?? [];
        var issues = new List<DataQualityIssue>();
        var types = new Dictionary<ValueType,int>();
        var valid = 0; var empty = 0;
        for (var i=0;i<list.Count;i++)
        {
            var raw = list[i];
            if (raw is null || string.IsNullOrWhiteSpace(raw.ToString())) { empty++; issues.Add(new("EMPTY", "Value is empty.", i, raw, ValueType.Unknown)); continue; }
            var result = SmartValue.Analyze(raw);
            types[result.Type] = types.TryGetValue(result.Type, out var count) ? count + 1 : 1;
            if (result.IsValid) valid++;
            else issues.AddRange(result.Errors.Select(e => new DataQualityIssue(e.Code,e.Message,i,raw,result.Type)));
        }
        var nonEmpty = list.Count - empty;
        return new(list.Count,valid,list.Count-valid-empty,empty,list.Count==0?1d:(double)nonEmpty/list.Count,nonEmpty==0?1d:(double)valid/nonEmpty,types,issues);
    }
}

public sealed record DatasetColumnProfile(string Name, int Count, int EmptyCount, ValueType DominantType, double TypeConfidence, double Completeness);
public sealed record DatasetSchema(IReadOnlyDictionary<string,ValueType> Columns, IReadOnlyList<DatasetColumnProfile> Profiles);

public static class DatasetAnalyzer
{
    public static IReadOnlyDictionary<string,ValueType> InferSchema(IEnumerable<IDictionary<string,object?>> rows)
    {
        var result = new Dictionary<string,ValueType>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in rows.SelectMany(r=>r).GroupBy(x=>x.Key,StringComparer.OrdinalIgnoreCase))
        {
            var candidates = group.Select(x=>Detector.Detect(x.Value?.ToString())).Where(x=>x.Type!=ValueType.Unknown).ToList();
            result[group.Key] = candidates.Count==0 ? ValueType.Unknown : candidates.GroupBy(x=>x.Type).OrderByDescending(g=>g.Average(x=>x.Confidence)).ThenByDescending(g=>g.Count()).First().Key;
        }
        return result;
    }

    public static DatasetSchema Analyze(IEnumerable<IDictionary<string,object?>> rows)
    {
        var materialized = rows.ToList();
        var names = materialized.SelectMany(x=>x.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
        var profiles = new List<DatasetColumnProfile>();
        foreach (var name in names)
        {
            var values = materialized.Select(r=>r.TryGetValue(name,out var v)?v:null).ToList();
            var nonEmpty = values.Where(v=>!string.IsNullOrWhiteSpace(v?.ToString())).ToList();
            var detections = nonEmpty.Select(v=>Detector.Detect(v?.ToString())).Where(x=>x.Type!=ValueType.Unknown).ToList();
            var dominantGroup = detections.Count == 0 ? null : detections.GroupBy(x => x.Type).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Average(x => x.Confidence)).First();
            var dominantType = dominantGroup?.Key ?? ValueType.Unknown;
            var dominantConfidence = dominantGroup?.Average(x => x.Confidence) ?? 0d;
            profiles.Add(new(name, values.Count, values.Count - nonEmpty.Count, dominantType, dominantConfidence, values.Count == 0 ? 1d : (double)nonEmpty.Count / values.Count));
        }
        return new(profiles.ToDictionary(x=>x.Name,x=>x.DominantType,StringComparer.OrdinalIgnoreCase),profiles);
    }
}

public sealed record DuplicateGroup<TKey>(TKey Key, IReadOnlyList<int> Indices);
public static class DuplicateDetector
{
    public static IReadOnlyList<DuplicateGroup<string>> Find<T>(IEnumerable<T> items, Func<T,object?> keySelector, ComparisonMode mode=ComparisonMode.Semantic)
    {
        return items.Select((item,index)=>(item,index,key:Canonicalizer.Create(keySelector(item)?.ToString(),mode)))
            .GroupBy(x=>x.key,StringComparer.Ordinal)
            .Where(g=>g.Count()>1)
            .Select(g=>new DuplicateGroup<string>(g.Key,g.Select(x=>x.index).ToList())).ToList();
    }
    public static IReadOnlyList<DuplicateGroup<string>> Find(IEnumerable<string?> values, ComparisonMode mode=ComparisonMode.Semantic)
        => Find(values,x=>x,mode);
}
