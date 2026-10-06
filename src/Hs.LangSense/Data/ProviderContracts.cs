namespace Hs.LangSense.Data;

public interface IExcelReader { IEnumerable<IDictionary<string, object?>> Read(Stream source, string? sheet = null); }
public interface IPdfTextReader { string ExtractText(Stream source); }
public interface IOcrEngine { OcrResult Recognize(Stream image); }
public sealed record OcrResult(string Text, double Confidence, IReadOnlyList<OcrToken> Tokens);
public sealed record OcrToken(string Text, double Confidence, int Start, int Length);
public interface IExternalDataSource { IAsyncEnumerable<IDictionary<string, object?>> ReadAsync(CancellationToken cancellationToken = default); }
