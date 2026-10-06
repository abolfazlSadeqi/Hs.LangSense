using System.Text.Json;
using System.Text.Json.Nodes;
namespace Hs.LangSense;
public static class JsonValueAnalyzer
{
    public static IReadOnlyList<TextOccurrence> Analyze(string json)
    {
        var node = JsonNode.Parse(json);
        var text = node?.ToJsonString() ?? json;
        return TextExtractor.Extract(text);
    }
    public static string Normalize(string json)
    {
        var node = JsonNode.Parse(json);
        return NormalizeNode(node).ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
    public static string Sanitize(string json) => SensitiveData.Mask(json);
    private static JsonNode NormalizeNode(JsonNode? node)
    {
        if (node is JsonValue v)
        {
            if (v.TryGetValue<string>(out var s))
                return JsonValue.Create(Normalizer.Normalize(s));
            return v.DeepClone();
        }
        if (node is JsonObject o)
        {
            var r = new JsonObject();
            foreach (var p in o) r[p.Key] = NormalizeNode(p.Value);
            return r;
        }
        if (node is JsonArray a)
        {
            var r = new JsonArray();
            foreach (var x in a) r.Add(NormalizeNode(x));
            return r;
        }
        return JsonValue.Create((string?)null)!;
    }
}
