using System.Collections;
using System.Reflection;
namespace Hs.LangSense;
public sealed record PropertyAnalysis(string Path, object? Raw, string Normalized, ValueType Type, double Confidence, bool IsValid, IReadOnlyList<ValidationIssue> Errors);
public static class ObjectAnalyzer
{
    public static IReadOnlyList<PropertyAnalysis> Analyze(object? value)
    {
        var r = new List<PropertyAnalysis>(); Walk(value, "", r, new HashSet<object>(ReferenceEqualityComparer.Instance)); return r;
    }
    private static void Walk(object? value, string path, List<PropertyAnalysis> r, HashSet<object> seen) { if (value is null) return; if (value is string or ValueType or decimal or int or long or double or float or DateTime or DateOnly or TimeOnly) { var x = SmartValue.Analyze(value); r.Add(new(path, value, x.NormalizedValue ?? "", x.Type, x.Confidence, x.IsValid, x.Errors)); return; } if (value is IEnumerable e) { var i = 0; foreach (var item in e) Walk(item, $"{path}[{i++}]", r, seen); return; } if (!value.GetType().IsValueType && !seen.Add(value)) return; foreach (var p in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanRead)) { var v = p.GetValue(value); var pp = string.IsNullOrEmpty(path) ? p.Name : $"{path}.{p.Name}"; if (v is string or ValueType or decimal or int or long or double or float or DateTime or DateOnly or TimeOnly) { var x = SmartValue.Analyze(v); r.Add(new(pp, v, x.NormalizedValue ?? "", x.Type, x.Confidence, x.IsValid, x.Errors)); } else Walk(v, pp, r, seen); } }
}
public static class ObjectNormalizer
{
    public static T Normalize<T>(T obj)
    {
        if (obj is null) return obj!;
        foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string)))
        {
            var v = (string?)p.GetValue(obj); if (v != null) p.SetValue(obj, Normalizer.Normalize(v));
        }
        return obj;
    }
}
