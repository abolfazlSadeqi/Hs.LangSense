namespace Hs.LangSense;

public enum ValueContext
{
    Generic, Money, Cheque, Year, PersianYear, GregorianYear, AccountNumber,
    Score, StudentScore, Percentage, Age, Phone, Card, IBAN, PostalCode,
    NationalCode, Email, Url
}

public sealed record ContextualValue(object Raw, ValueContext Context)
{
    public ValueResult<string> Analyze()
    {
        var text = Raw?.ToString() ?? string.Empty;
        switch (Context)
        {
            case ValueContext.Money:
                return Money.TryParse(text, out var money)
                    ? Result(money.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture), ValueType.Money, money.Currency, 0.99)
                    : Invalid(ValueType.Money);
            case ValueContext.Cheque:
                return Cheque.TryParse(text, out var cheque)
                    ? Result(cheque.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture), ValueType.Cheque, cheque.Currency, 0.98)
                    : Invalid(ValueType.Cheque);
            case ValueContext.Percentage:
                return PercentageParser.TryParse(text, out var percentage) && percentage is >= 0 and <= 100
                    ? Result(percentage.ToString(System.Globalization.CultureInfo.InvariantCulture), ValueType.Percentage, null, 0.98)
                    : Invalid(ValueType.Percentage);
            case ValueContext.Score:
            case ValueContext.StudentScore:
                return ScoreParser.TryParse(text, out var score, out var maximum) && score >= 0 && score <= maximum
                    ? Result(score.ToString(System.Globalization.CultureInfo.InvariantCulture), ValueType.Score, maximum, 0.95)
                    : Invalid(ValueType.Score);
            case ValueContext.Age:
                return decimal.TryParse(Normalizer.Normalize(text), out var age) && age is >= 0 and <= 150
                    ? Result(age.ToString(System.Globalization.CultureInfo.InvariantCulture), ValueType.Age, null, 0.95)
                    : Invalid(ValueType.Age);
            case ValueContext.Phone:
                return IranianMobile.TryNormalize(text, out var phone)
                    ? Result(phone, ValueType.IranianMobile, new { International = IranianMobile.ToInternational(phone) }, 0.99)
                    : Invalid(ValueType.IranianMobile);
            case ValueContext.Card:
                return BankCard.IsValid(text) ? Result(BankCard.Normalize(text), ValueType.BankCard, BankCard.DetectBank(text), 0.99) : Invalid(ValueType.BankCard);
            case ValueContext.IBAN:
                return Iban.IsValid(text) ? Result(Iban.Normalize(text), ValueType.IBAN, Iban.BankCode(text), 0.99) : Invalid(ValueType.IBAN);
            case ValueContext.AccountNumber:
                return AccountNumber.IsValid(text) ? Result(AccountNumber.Normalize(text), ValueType.AccountNumber, null, 0.9) : Invalid(ValueType.AccountNumber);
            case ValueContext.PostalCode:
                return IranianPostalCode.IsValid(text) ? Result(IranianPostalCode.Normalize(text), ValueType.PostalCode, null, 0.95) : Invalid(ValueType.PostalCode);
            case ValueContext.NationalCode:
                return IranianNationalCode.IsValid(text) ? Result(IranianNationalCode.Normalize(text), ValueType.NationalCode, null, 0.99) : Invalid(ValueType.NationalCode);
            case ValueContext.Email:
                return EmailValue.TryParse(text, out var email) ? Result(email.Address, ValueType.Email, email.Domain, 0.98) : Invalid(ValueType.Email);
            case ValueContext.Url:
                return UrlValue.TryParse(text, out var url) ? Result(url.Value, ValueType.Url, url.Host, 0.98) : Invalid(ValueType.Url);
            case ValueContext.PersianYear:
                return YearParser.IsPersianYear(text) ? Result(Normalizer.Normalize(text), ValueType.PersianDate, null, 0.95) : Invalid(ValueType.PersianDate);
            case ValueContext.GregorianYear:
                return YearParser.IsGregorianYear(text) ? Result(Normalizer.Normalize(text), ValueType.GregorianDate, null, 0.95) : Invalid(ValueType.GregorianDate);
            case ValueContext.Year:
                return YearParser.IsPersianYear(text) || YearParser.IsGregorianYear(text) ? Result(Normalizer.Normalize(text), ValueType.Year, null, 0.9) : Invalid(ValueType.Year);
            default:
                return SmartValue.Analyze(Raw);
        }
    }

    private ValueResult<string> Result(string value, ValueType type, object? metadata, double confidence)
    {
        var result = new ValueResult<string>
        {
            Success = true, IsValid = true, Value = value, RawValue = Raw,
            NormalizedValue = value, CanonicalValue = Canonicalizer.Create(value), Type = type, Confidence = confidence
        };
        if (metadata is not null) result.Metadata["Context"] = metadata;
        return result;
    }

    private static ValueResult<string> Invalid(ValueType type) => new()
    {
        Success = false, IsValid = false, Type = type,
        Errors = new List<ValidationIssue> { new("INVALID_CONTEXT_VALUE", "Value is not valid for selected context.") }
    };
}

public static class ContextExtensions
{
    public static ContextualValue As(this object value, ValueContext context) => new(value, context);
    public static ValueResult<string> AnalyzeAs(this object value, ValueContext context) => new ContextualValue(value, context).Analyze();
    public static string FormatAs(this object value, ValueContext context)
    {
        var result = new ContextualValue(value, context).Analyze();
        return result.Success ? result.Value ?? string.Empty : value?.ToString() ?? string.Empty;
    }
}
