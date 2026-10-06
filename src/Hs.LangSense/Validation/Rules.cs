namespace Hs.LangSense;
public interface IRule
{
    string Name { get; }
    ValidationIssue? Validate(string value); string Explain();
}
public sealed class DelegateRule(string name, Func<string, bool> predicate, string explanation) : IRule
{
    public string Name => name; public ValidationIssue? Validate(string value) => predicate(value) ? null : new("RULE_FAILED", explanation, true, name); public string Explain() => explanation;
}
public sealed class CompositeRule : IRule
{
    private readonly bool _all; private readonly IReadOnlyList<IRule> _rules; public CompositeRule(bool all, IEnumerable<IRule> rules) { _all = all; _rules = rules.ToList(); }
    public string Name => _all ? "All" : "Any"; public ValidationIssue? Validate(string v) { var r = _rules.Select(x => x.Validate(v)).Where(x => x != null).Cast<ValidationIssue>().ToList(); return _all ? (r.Count == 0 ? null : new("COMPOSITE_FAILED", string.Join("; ", r.Select(x => x.Message)), true, Name)) : (r.Count < _rules.Count ? null : new("COMPOSITE_FAILED", "No alternative rule passed.", true, Name)); }
    public string Explain() => string.Join(_all ? " AND " : " OR ", _rules.Select(x => x.Explain()));
}
public static class RuleExtensions
{
    public static IRule And(this IRule a, IRule b) => new CompositeRule(true, [a, b]); public static IRule Or(this IRule a, IRule b) => new CompositeRule(false, [a, b]);
}
public static class Rules
{
    public static IRule Required =>
        new DelegateRule("Required", s => !string.IsNullOrWhiteSpace(s), "Value is required.");
    public static IRule Numeric =>
        new DelegateRule("Numeric", s => NumberParser.TryParse(s, out _), "Value must be numeric.");
    public static IRule Integer =>
        new DelegateRule("Integer", s => long.TryParse(Normalizer.Normalize(s), out _), "Value must be an integer.");
    public static IRule Decimal =>
        new DelegateRule("Decimal", s => decimal.TryParse(Normalizer.Normalize(s), out _), "Value must be decimal.");
    public static IRule Positive => new DelegateRule("Positive", s => decimal.TryParse(Normalizer.Normalize(s), out var x) && x > 0, "Value must be positive.");
    public static IRule NonNegative => new DelegateRule("NonNegative", s => decimal.TryParse(Normalizer.Normalize(s), out var x) && x >= 0, "Value must be non-negative.");
    public static IRule Range(decimal min, decimal max) => new DelegateRule("Range", s => decimal.TryParse(Normalizer.Normalize(s), out var x) && x >= min && x <= max, $"Value must be between {min} and {max}.");
    public static IRule Length(int min, int max) => new DelegateRule("Length", s => s.Length >= min && s.Length <= max, $"Length must be between {min} and {max}.");
    public static IRule Regex(string pattern) => new DelegateRule("Regex", s => System.Text.RegularExpressions.Regex.IsMatch(s, pattern), $"Value must match {pattern}.");
    public static IRule Percentage => new DelegateRule("Percentage", s => PercentageParser.TryParse(s, out var x) && x >= 0 && x <= 100, "Percentage must be between 0 and 100.");
    public static IRule StudentScore => new DelegateRule("StudentScore", s => decimal.TryParse(Normalizer.Normalize(s), out var x) && x >= 0 && x <= 20, "Student score must be between 0 and 20.");
    public static IRule Age => new DelegateRule("Age", s => decimal.TryParse(Normalizer.Normalize(s), out var x) && x >= 0 && x <= 150, "Age must be between 0 and 150.");
    public static IRule Mobile => new DelegateRule("Mobile", IranianMobile.IsValid, "Invalid Iranian mobile.");
    public static IRule NationalCode => new DelegateRule("NationalCode", IranianNationalCode.IsValid, "Invalid Iranian national code.");
    public static IRule LegalNationalCode => new DelegateRule("LegalNationalCode", LegalNationalCodeValue.IsValid, "Invalid legal national identifier.");
    public static IRule PostalCode => new DelegateRule("PostalCode", IranianPostalCode.IsValid, "Invalid postal code.");
    public static IRule BankCard => new DelegateRule("BankCard", Hs.LangSense.BankCard.IsValid, "Invalid bank card.");
    public static IRule IBAN => new DelegateRule("IBAN", Iban.IsValid, "Invalid IBAN.");
    public static IRule Email => new DelegateRule("Email", EmailValue.IsValid, "Invalid email.");
    public static IRule Url => new DelegateRule("Url", UrlValue.IsValid, "Invalid URL.");
    public static IRule Money => new DelegateRule("Money", s => Hs.LangSense.Money.TryParse(s, out _), "Invalid money value.");
    public static IRule PersianText => new DelegateRule("PersianText", s => s.Any(Hs.LangSense.PersianText.IsPersianLetter), "Value should contain Persian letters.");
}
public sealed class RuleSet
{
    private readonly List<IRule> _rules = []; public RuleSet Add(IRule r) { _rules.Add(r); return this; }
    public IReadOnlyList<ValidationIssue> Validate(string value) => _rules.Select(r => r.Validate(value)).Where(x => x != null).Cast<ValidationIssue>().ToList(); public string Explain() => string.Join(Environment.NewLine, _rules.Select(x => $"{x.Name}: {x.Explain()}")); public bool IsValid(string value) => Validate(value).Count == 0;
}
public static class ValidationEngine
{
    public static ValueResult<string> Validate(string value, params IRule[] rules)
    {
        var normalized = Normalizer.Normalize(value);
        var issues = rules.Select(x => x.Validate(normalized)).Where(x => x != null).Cast<ValidationIssue>().ToList(); return new() { Success = issues.Count == 0, Value = normalized, RawValue = value, NormalizedValue = normalized, IsValid = issues.Count == 0, Errors = issues, Type = ValueType.Unknown };
    }
}
