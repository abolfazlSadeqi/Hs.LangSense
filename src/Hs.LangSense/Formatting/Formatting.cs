namespace Hs.LangSense;
public static class Formatter
{
    public static string Format(string value, string pattern)
    {
        var chars = value.ToCharArray(); var i = 0;
        return new string(pattern.Select(c => c == '#' && i < chars.Length ? chars[i++] : c).ToArray());
    }
    public static string Money(decimal amount, Currency currency = Currency.Toman) => Hs.LangSense.Money.Format(amount, currency);
    public static string Mobile(string value) => IranianMobile.Format(value);
    public static string Card(string value) => BankCard.Format(value);
    public static string IBAN(string value) => Iban.Format(value);
}
