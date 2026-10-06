namespace Hs.LangSense;

public static class Parse
{
    public static bool Number(string? value, out decimal result) => NumberParser.TryParse(value, out result);
    public static bool Money(string? value, out MoneyValue result) => Hs.LangSense.Money.TryParse(value, out result);
    public static bool Cheque(string? value, out ChequeValue result) => Hs.LangSense.Cheque.TryParse(value, out result);
    public static bool Mobile(string? value, out IranianMobileValue result) => IranianMobile.TryParse(value, out result);
    public static bool Email(string? value, out EmailValue result) => EmailValue.TryParse(value, out result);
    public static bool Url(string? value, out UrlValue result) => UrlValue.TryParse(value, out result);
    public static bool Phone(string? value, out PhoneValue result) => PhoneParser.TryParse(value, out result);
    public static bool Date(string? value, out ParsedDate result) => DateParser.TryParse(value, out result);
    public static bool Time(string? value, out TimeSpan result) => TimeParser.TryParse(value, out result);
    public static bool DateTime(string? value, out ParsedDateTime result) => DateTimeParser.TryParse(value, out result);
}
