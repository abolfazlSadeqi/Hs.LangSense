namespace Hs.LangSense;
public abstract record StrongValue<T>(T Value)
{
    public override string ToString() => Value is null ? "" : Value.ToString() ?? "";
}
public sealed record MobileValue(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.IranianMobile.IsValid(Value);
    public string International => Hs.LangSense.IranianMobile.ToInternational(Value);
    public string Formatted => Hs.LangSense.IranianMobile.Format(Value);
    public static bool TryParse(string s, out MobileValue value)
    {
        value = new(Hs.LangSense.IranianMobile.TryNormalize(s, out var n) ? n : s); return value.IsValid;
    }
}
public sealed record NationalCode(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.IranianNationalCode.IsValid(Value); public static bool TryParse(string s, out NationalCode value) { value = new(Hs.LangSense.IranianNationalCode.Normalize(s)); return value.IsValid; }
}
public sealed record PostalCode(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.IranianPostalCode.IsValid(Value); public static bool TryParse(string s, out PostalCode value) { value = new(Hs.LangSense.IranianPostalCode.Normalize(s)); return value.IsValid; }
}
public sealed record BankCardNumber(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => BankCard.IsValid(Value); public string Formatted => BankCard.Format(Value); public string Masked => BankCard.Mask(Value); public static bool TryParse(string s, out BankCardNumber value) { value = new(BankCard.Normalize(s)); return value.IsValid; }
}
public sealed record IbanValue(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Iban.IsValid(Value); public string Formatted => Iban.Format(Value); public static bool TryParse(string s, out IbanValue value) { value = new(Iban.Normalize(s)); return value.IsValid; }
}
public sealed record MoneyValueStrong(decimal Amount, Currency Currency);
public sealed record PercentageValue(decimal Value)
{
    public bool IsValid => Value >= 0 && Value <= 100;
}
public sealed record StudentScore(decimal Value, decimal Maximum = 20)
{
    public bool IsValid => Value >= 0 && Value <= Maximum;
}

public sealed record AccountNumberValue(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.AccountNumber.IsValid(Value);
    public string Masked => Hs.LangSense.AccountNumber.Mask(Value);
}
public sealed record SayadValue(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.Sayad.IsValid(Value);
}
public sealed record ChequeNumberValue(string Value) : StrongValue<string>(Value)
{
    public bool IsValid => Hs.LangSense.ChequeNumber.IsValid(Value);
}
public sealed record ChequeAmount(decimal Amount, Currency Currency = Currency.Toman)
{
    public string Words => Cheque.ToWords(Amount);
    public string Formatted => Cheque.Format(Amount, Currency);
}
