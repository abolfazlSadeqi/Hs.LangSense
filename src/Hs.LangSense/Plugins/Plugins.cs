namespace Hs.LangSense;
public interface ICustomDetector { DetectionCandidate Detect(string value); }
public interface ICustomNormalizer { string Normalize(string value); }
public interface ICustomValidator { ValidationIssue? Validate(string value); }
public interface ICustomFormatter { string Format(string value); }
public interface ICustomExtractor { IEnumerable<TextOccurrence> Extract(string text); }
public interface ICustomAnalyzer { object Analyze(string value); }
public static class PluginRegistry
{
    public static readonly List<ICustomDetector> Detectors = [];
    public static readonly List<ICustomNormalizer> Normalizers = [];
    public static readonly List<ICustomValidator> Validators = [];
    public static readonly List<ICustomFormatter> Formatters = [];

    public static void Register(ICustomDetector x) => Detectors.Add(x);
    public static void Register(ICustomNormalizer x) => Normalizers.Add(x);
    public static void Register(ICustomValidator x) => Validators.Add(x);
    public static void Register(ICustomFormatter x) => Formatters.Add(x);
}
