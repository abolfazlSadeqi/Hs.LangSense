using System.Text.RegularExpressions;

namespace Hs.LangSense;

public sealed record PhoneValue(string National, string International, string CountryCode, string Prefix, bool IsValid);

public static class PhoneParser
{
    public static bool TryParse(string? input, out PhoneValue value)
    {
        value = new PhoneValue(string.Empty, string.Empty, string.Empty, string.Empty, false);
        var s = Normalizer.Normalize(input).Trim();
        if (IranianMobile.TryNormalize(s, out var national))
        {
            value = new PhoneValue(national, "+98" + national[1..], "98", national[..4], true);
            return true;
        }
        var generic = Regex.Replace(s, @"[\s()\-]", string.Empty);
        if (Regex.IsMatch(generic, @"^\+[1-9]\d{7,14}$"))
        {
            var digits = generic[1..];
            value = new PhoneValue(generic, generic, digits[..Math.Min(3,digits.Length)], digits.Length >= 4 ? digits[..4] : digits, true);
            return true;
        }
        return false;
    }
}
