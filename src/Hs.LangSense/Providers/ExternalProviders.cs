namespace Hs.LangSense;

public interface IExcelDataSourceAdapter : IDataSourceAdapter { }
public interface IPdfTextSourceAdapter
{
    string ExtractText(Stream stream, string? fileName = null);
}
public interface IOcrEngine
{
    OcrCleanResult RecognizeAndClean(Stream image, string? fileName = null);
}

public sealed class CompositeImportProvider
{
    private readonly List<IDataSourceAdapter> _sources = new();
    public CompositeImportProvider Add(IDataSourceAdapter adapter) { _sources.Add(adapter); return this; }
    public IEnumerable<IDictionary<string,object?>> Read(Stream stream, string? fileName = null)
    {
        var adapter = _sources.FirstOrDefault();
        return adapter is null ? Array.Empty<IDictionary<string,object?>>() : adapter.Read(stream, fileName);
    }
}
