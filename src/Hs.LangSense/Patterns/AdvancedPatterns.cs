using System.Text.RegularExpressions;

namespace Hs.LangSense;

public static class PatternComposition
{
    public static ValuePattern Combine(string name, ValueType type, params ValuePattern[] patterns)
    {
        var expression = string.Join("|", patterns.Select(x => $"(?:{x.Regex.ToString()})"));
        return new ValuePattern(name, expression, type);
    }

    public static IReadOnlyList<PatternMatch> Extract(ValuePattern pattern, string text) => pattern.Extract(text).ToList();
}

public static class PatternLibrary
{
    public static ValuePattern PersianNationalCode =>
        new("PersianNationalCode", @"(?<!\d)[۰-۹]{10}(?!\d)", ValueType.NationalCode);
    public static ValuePattern ArabicNationalCode =>
        new("ArabicNationalCode", @"(?<!\d)[٠-٩]{10}(?!\d)", ValueType.NationalCode);
    public static ValuePattern PersianMobile =>
        new("PersianMobile", @"(?<!\d)۰۹[۰-۹]{9}(?!\d)", ValueType.IranianMobile);
    public static ValuePattern PersianDate =>
        new("PersianDate", @"(?<!\d)1[34][0-9]{2}[/-][0-9]{1,2}[/-][0-9]{1,2}(?!\d)", ValueType.PersianDate);
    public static ValuePattern Time =>
        new("Time", @"(?<!\d)(?:[01]?\d|2[0-3]):[0-5]\d(?::[0-5]\d)?(?!\d)", ValueType.Time);
    public static ValuePattern UUID =>
        new("UUID", @"(?i)(?<![0-9a-f])[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}(?![0-9a-f])", ValueType.Identifier);
}
