using System.Globalization;

namespace Hs.LangSense;

/// <summary>Entry point for language and value intelligence operations.</summary>
public static class LangSense
{
    /// <summary>Analyzes a value through normalization, detection, context and validation.</summary>
    public static ValueResult<string> Analyze(object? value, LangSenseOptions? options = null, ValueContext? context = null)
    {
        var smart = SmartValue.From(value).Normalize(options);
        if (context.HasValue)
        {
            var contextual = smart.As(context.Value).Analyze();
            if (contextual.Success) return contextual;
        }
        return smart.Detect(context).Canonicalize().Build();
    }

    /// <summary>Analyzes a sequence of values using the same options.</summary>
    public static IReadOnlyList<ValueResult<string>> AnalyzeMany(IEnumerable<object?> values, LangSenseOptions? options = null)
        => values.Select(x => Analyze(x, options)).ToList();

    /// <summary>Normalizes a value using the configured multilingual normalization rules.</summary>
    public static string Normalize(string? value, LangSenseOptions? options = null) => Normalizer.Normalize(value, options);
    /// <summary>Detects the most likely semantic type of a value.</summary>
    public static DetectionResult Detect(string? value, object? context = null) => Detector.Detect(value, context);
    /// <summary>Validates a string against composable rules.</summary>
    public static ValueResult<string> Validate(string value, params IRule[] rules) => ValidationEngine.Validate(value, rules);
    /// <summary>Creates a canonical representation for comparison and search.</summary>
    public static string Canonicalize(string? value, ComparisonMode mode = ComparisonMode.Semantic) => Canonicalizer.Create(value, mode);
    /// <summary>Masks supported sensitive values.</summary>
    public static string Mask(string? value) => SensitiveData.Mask(value ?? string.Empty);
    /// <summary>Redacts supported sensitive values.</summary>
    public static string Redact(string? value) => SensitiveData.Redact(value ?? string.Empty);
}

public static class ValueResultExtensions
{
    public static ValueResult<T> WithSuggestion<T>(this ValueResult<T> result, string suggestion)
    {
        result.Suggestions.Add(suggestion);
        return result;
    }

    public static ValueResult<T> WithMetadata<T>(this ValueResult<T> result, string key, object? value)
    {
        result.Metadata[key] = value;
        return result;
    }

    public static ValueResult<T> ThrowIfInvalid<T>(this ValueResult<T> result)
    {
        if (!result.IsValid) throw new FormatException(result.Explain());
        return result;
    }
}
