namespace Hs.LangSense;

public static class RuleCombinators
{
    public static IRule All(params IRule[] rules) => new CompositeRule(true, rules);
    public static IRule Any(params IRule[] rules) => new CompositeRule(false, rules);
    public static IRule Not(IRule rule) => new DelegateRule($"NOT({rule.Name})", x => rule.Validate(x) is null, $"Value must not satisfy {rule.Name}.");
}

public static class ValidationExplain
{
    public static string Explain(string value, params IRule[] rules)
        => string.Join(Environment.NewLine, rules.Select(rule =>
        {
            var issue = rule.Validate(value);
            return issue is null ? $"✓ {rule.Name}: valid" : $"✗ {rule.Name}: {issue.Message}";
        }));
}

public static class ValidationProfiles
{
    public static RuleSet IranianIdentity() => new RuleSet().Add(Rules.Required).Add(Rules.NationalCode);
    public static RuleSet IranianContact() => new RuleSet().Add(Rules.Required).Add(Rules.Mobile);
    public static RuleSet PaymentCard() => new RuleSet().Add(Rules.Required).Add(Rules.BankCard);
    public static RuleSet FinancialAccount() => new RuleSet().Add(Rules.Required).Add(Rules.IBAN);
    public static RuleSet PersianScore() => new RuleSet().Add(Rules.Required).Add(Rules.StudentScore);
}
