using System.Globalization;

namespace Hs.LangSense;

public sealed record Transformation(TransformationKind Kind, string Name, string Before, string After, bool Safe = true, string? Reason = null);
public sealed record ValidationIssue(string Code, string Message, bool IsError = true, string? Rule = null, int? Position = null);
public sealed record DetectionCandidate(ValueType Type, double Confidence, bool IsValid, string? Reason = null);
public sealed record RepairSuggestion(string Current, string Suggested, bool Safe, string Reason, double Confidence = 1);
public sealed record TextOccurrence(string Value, string Normalized, ValueType Type, int Start, int Length, double Confidence, bool IsValid, IReadOnlyList<ValidationIssue> Issues);
public sealed record LanguageDistribution(Language Language, double Percentage, int Characters);
public sealed record CharacterInfo(string Character, int CodePoint, string UnicodeName, System.Globalization.UnicodeCategory Category, string Script, Language Language, bool IsLetter, bool IsDigit, bool IsWhitespace, bool IsInvisible, int Index);

public sealed class ValueResult<T>
{
    public bool Success { get; set; }
    public T? Value { get; set; }
    public object? RawValue { get; set; }
    public string? NormalizedValue { get; set; }
    public string? CanonicalValue { get; set; }
    public ValueType Type { get; set; }
    public double Confidence { get; set; }
    public bool IsValid { get; set; }
    public List<ValidationIssue> Errors { get; set; } = [];
    public List<ValidationIssue> Warnings { get; set; } = [];
    public List<string> Suggestions { get; set; } = [];
    public Dictionary<string, object?> Metadata { get; set; } = [];
    public List<Transformation> Transformations { get; set; } = [];
    public List<DetectionCandidate> Alternatives { get; set; } = [];
    public string Explain() {
        var lines = new List<string> { $"Type: {Type}", $"Raw: {RawValue}", $"Normalized: {NormalizedValue}", $"Canonical: {CanonicalValue}", $"Valid: {IsValid}", $"Confidence: {Confidence:P0}" };
        if (Errors.Count > 0) lines.AddRange(Errors.Select(x => $"✗ {x.Message}"));
        if (Warnings.Count > 0) lines.AddRange(Warnings.Select(x => $"⚠ {x.Message}"));
        if (Suggestions.Count > 0) { lines.Add("Suggestions:"); lines.AddRange(Suggestions.Select(x => $"→ {x}")); }
        if (Transformations.Count > 0) { lines.Add("Transformations:"); lines.AddRange(Transformations.Select(x => $"→ {x.Name}: {x.Before} => {x.After}")); }
        return string.Join(Environment.NewLine, lines);
    }
}

public sealed class LangSenseOptions
{
    public ValidationMode ValidationMode { get; set; } = ValidationMode.Normalized;
    public bool ConvertPersianDigitsToLatin { get; set; } = true;
    public bool ConvertArabicDigitsToLatin { get; set; } = true;
    public bool NormalizeArabicLetters { get; set; } = true;
    public bool NormalizeWhitespace { get; set; } = true;
    public bool NormalizePunctuation { get; set; } = true;
    public bool RemoveControlCharacters { get; set; }
    public bool PreserveZeroWidthNonJoiner { get; set; } = false;
    public bool PreserveZeroWidthJoiner { get; set; } = false;
    public bool PreserveLeftRightMarks { get; set; } = false;
    public bool RemoveBom { get; set; } = true;
    public bool NormalizeArabicIndicDigits { get; set; } = true;
    public bool NormalizePersianLetters { get; set; } = true;
    public bool NormalizeTatweel { get; set; } = true;
    public bool RemoveDiacritics { get; set; }
    public bool Trim { get; set; } = true;
    public bool NormalizeSigns { get; set; } = true;
    public bool EnableHeuristicDetection { get; set; } = true;
    public NumberSystem OutputNumberSystem { get; set; } = NumberSystem.Latin;
}

public sealed class SmartValue
{
    public object? Raw { get; }
    public string Text { get; }
    public string Normalized { get; private set; }
    public string Canonical { get; private set; } = "";
    public ValueType Type { get; private set; } = ValueType.Unknown;
    public double Confidence { get; private set; }
    public bool IsValid { get; private set; }
    public List<Transformation> Transformations { get; } = [];
    public List<ValidationIssue> Errors { get; } = [];
    public List<ValidationIssue> Warnings { get; } = [];
    public List<DetectionCandidate> Alternatives { get; private set; } = [];
    private SmartValue(object? raw) { Raw = raw; Text = Convert.ToString(raw, CultureInfo.InvariantCulture) ?? ""; Normalized = Text; }
    public static SmartValue From(object? value) => new(value);
    public static SmartValue Parse(object? value) => new(value);
    public static bool TryParse(object? value, out SmartValue result) { result = new(value); result.Normalize().Detect().Canonicalize(); return result.IsValid; }
    public SmartValue As(ValueType context) { Type=context; return this; }
    public ContextualValue As(ValueContext context) => new(Raw ?? Text, context);
    public SmartValue Validate(params IRule[] rules) { var issues=rules.Select(x=>x.Validate(Normalized)).Where(x=>x!=null).Cast<ValidationIssue>().ToList(); Errors.Clear(); Errors.AddRange(issues); IsValid=issues.Count==0; return this; }
    public SmartValue Format(string pattern) { Normalized=Formatter.Format(Normalized,pattern); return this; }
    public SmartValue Mask(MaskType type=MaskType.Generic) { Normalized=type switch { MaskType.Mobile=>IranianMobile.Mask(Normalized), MaskType.BankCard=>BankCard.Mask(Normalized), MaskType.IBAN=>Iban.Mask(Normalized), MaskType.NationalCode=>IranianNationalCode.Mask(Normalized), MaskType.Email=>SensitiveData.Mask(Normalized), _=>MaskEngine.Mask(Normalized,"****") }; return this; }
    public ValueResult<string> Analyze() => Detect().Canonicalize().Build();
    public SmartValue Normalize(LangSenseOptions? options = null) { var r = Normalizer.NormalizeDetailed(Normalized, options); Normalized = r.Value; Transformations.AddRange(r.Transformations); return this; }
    public SmartValue Detect(object? context = null) { var r = Detector.Detect(Normalized, context); Type=r.Type; Confidence=r.Confidence; IsValid=r.IsValid; Alternatives=r.Alternatives; return this; }
    public SmartValue Canonicalize(ComparisonMode mode=ComparisonMode.Semantic) { Canonical=Canonicalizer.Create(Normalized, mode); return this; }
    public ValueResult<string> Build() => new() { Success=true, Value=Normalized, RawValue=Raw, NormalizedValue=Normalized, CanonicalValue=Canonical.Length>0?Canonical:Canonicalizer.Create(Normalized), Type=Type, Confidence=Confidence, IsValid=IsValid, Transformations=[..Transformations], Alternatives=[..Alternatives], Errors=[..Errors], Warnings=[..Warnings] };
    public static ValueResult<string> Analyze(object? value, LangSenseOptions? options=null, object? context=null) => From(value).Normalize(options).Detect(context).Canonicalize().Build();
    public static IReadOnlyList<ValueResult<string>> AnalyzeMany(IEnumerable<object?> values, LangSenseOptions? options=null) => values.Select(x=>Analyze(x,options)).ToList();
}
