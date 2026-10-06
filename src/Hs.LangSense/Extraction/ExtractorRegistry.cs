namespace Hs.LangSense;
public static class ExtractorRegistry
{
    private static readonly List<ICustomExtractor> Items = [];
    public static void Register(ICustomExtractor x) => Items.Add(x);
    public static IReadOnlyList<TextOccurrence>
        Extract(string text) => Items.SelectMany(x => x.Extract(text)).ToList();
}
