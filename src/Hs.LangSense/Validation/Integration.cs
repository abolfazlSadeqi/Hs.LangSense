namespace Hs.LangSense;
public interface IFluentValidationRuleAdapter
{
    void Configure<T>(object ruleBuilder);
}
public static class FluentValidationExpressions
{
    public static Func<string, bool> MustBeIranianMobile => IranianMobile.IsValid;
    public static Func<string, bool> MustBeNationalCode => IranianNationalCode.IsValid;
    public static Func<string, bool> MustBeIBAN => Iban.IsValid;
    public static Func<string, bool> MustBeBankCard => BankCard.IsValid;
    public static Func<decimal, bool> BeStudentScore => x => x >= 0 && x <= 20;
    public static Func<decimal, bool> BePercentage => x => x >= 0 && x <= 100;
}
