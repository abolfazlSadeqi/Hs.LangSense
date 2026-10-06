namespace Hs.LangSense;
public static class AccountNumber
{
    public static string Normalize(string value) => Normalizer.Normalize(value).Replace("-", "").Replace(" ", "");
    public static bool IsValid(string value, int min = 4, int max = 30)
    {
        var s = Normalize(value);
        return s.Length >= min && s.Length <= max && s.All(char.IsDigit);
    }
    public static string Mask(string value)
    {

        var s = Normalize(value); return s.Length <= 4 ? new string('*', s.Length) : new string('*', s.Length - 4) + s[^4..];
    }
}
