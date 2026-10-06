using System.Text.RegularExpressions;
namespace Hs.LangSense;
public static class IranianMobile
{
    public static bool TryParse(string? input, out IranianMobileValue value)
    {
        var n = Normalizer.Normalize(input);
        n = Regex.Replace(n, @"[\s\-()]+", ""); if (n.StartsWith("0098")) n = "+" + n[2..];
        if (n.StartsWith("98") && !n.StartsWith("+")) n = "+" + n; if (n.StartsWith("+98")) n = "0" + n[3..];
        value = new IranianMobileValue(n); return IsValid(n);
    }
    public static bool IsValid(string? input) => TryNormalize(input, out _);
    public static bool TryNormalize(string? input, out string normalized)
    {
        normalized = ""; var s = Normalizer.Normalize(input); s = Regex.Replace(s, @"[\s\-()]+", "");
        if (s.StartsWith("+989")) s = "0" + s[3..];
        else if (s.StartsWith("00989")) s = "0" + s[4..];
        else if (s.StartsWith("989")) s = "0" + s[2..];
        normalized = s;
        return Regex.IsMatch(s, @"^09\d{9}$");
    }
    public static string ToInternational(string input) => TryNormalize(input, out var n) ? "+98" + n[1..] : input;
    public static string Format(string input) => TryNormalize(input, out var n) ? $"{n[..4]}-{n[4..7]}-{n[7..]}" : input;
    public static string Mask(string input) => TryNormalize(input, out var n) ? $"{n[..4]}***{n[^4..]}" : "***";
}
public sealed record IranianMobileValue(string National)
{

    public string International => IranianMobile.ToInternational(National);
    public string Formatted => IranianMobile.Format(National);
    public string Masked => IranianMobile.Mask(National); public string Prefix => National.Length >= 4 ? National[..4] : "";
    public bool IsValid => IranianMobile.IsValid(National);
}
public static class IranianNationalCode
{
    public static bool IsValid(string? input)
    {
        var s = Normalizer.Normalize(input);
        if (!Regex.IsMatch(s, @"^\d{10}$") || s.Distinct().Count() == 1) return false;
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (s[i] - '0') * (10 - i);
        var r = sum % 11; var c = s[9] - '0';
        return r < 2 ? c == r : c == 11 - r;
    }
    public static string Normalize(string input) => Normalizer.Normalize(input).PadLeft(10, '0');
    public static string Format(string input) => Normalize(input);
    public static string Mask(string input)
    {
        var s = Normalize(input); return s[..3] + "****" + s[^3..];
    }
}
public static class LegalNationalCodeValue
{
    public static bool IsValid(string? input)
    {
        var s = Normalizer.Normalize(input);
        if (!Regex.IsMatch(s, @"^\d{11}$"))
            return false;
        if (s.Distinct().Count() == 1) return false; var weights = new[] { 29, 27, 23, 19, 17, 29, 27, 23, 19, 17 };
        var sum = 0; for (int i = 0; i < 10; i++) sum += (s[i] - '0') * weights[i]; var remainder = sum % 11;

        var check = remainder < 2 ? remainder : 11 - remainder; return s[10] - '0' == check;
    }
}
public static class IranianPostalCode
{
    public static bool IsValid(string? input)
    {
        var s = Normalizer.Normalize(input).Replace("-", "");
        return Regex.IsMatch(s, @"^\d{10}$") && !s.All(c => c == s[0]);
    }
    public static string Normalize(string input) => Normalizer.Normalize(input).Replace("-", "").Replace(" ", "");
}
public static class BankCard
{
    public static bool IsValid(string? input) { var s = Normalizer.Normalize(input).Replace("-", "").Replace(" ", ""); if (!Regex.IsMatch(s, @"^\d{16}$")) return false; var sum = 0; for (int i = 0; i < 16; i++) { var d = s[i] - '0'; if (i % 2 == 0) { d *= 2; if (d > 9) d -= 9; } sum += d; } return sum % 10 == 0; }
    public static string Normalize(string input) => Normalizer.Normalize(input).Replace("-", "").Replace(" ", ""); public static string Format(string input) { var s = Normalize(input); return Regex.IsMatch(s, @"^\d{16}$") ? $"{s[..4]}-{s[4..8]}-{s[8..12]}-{s[12..]}" : input; }
    public static string Mask(string input)
    {
        var s = Normalize(input);
        return s.Length == 16 ? $"{s[..4]}-****-****-{s[^4..]}" : "****";
    }
    public static string? DetectBank(string input)
    {
        var s = Normalize(input);
        return s.Length >= 6 ? BankCatalog.TryGetName(s[..6]) : null;
    }
}
public static class Iban
{
    public static bool IsValid(string? input)
    {

        var s = Normalizer.Normalize(input).Replace(" ", "").ToUpperInvariant();
        if (!Regex.IsMatch(s, @"^IR\d{24}$")) return false; var rearr = s[4..] + s[..4];
        var numeric = new string(rearr.Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString())
            .SelectMany(x => x).ToArray());
        int rem = 0; foreach (var ch in numeric) rem = (rem * 10 + (ch - '0')) % 97; return rem == 1;
    }
    public static string Normalize(string input) => Normalizer.Normalize(input)
        .Replace(" ", "").ToUpperInvariant();
    public static string Format(string input)
    {
        var s = Normalize(input);
        return s.Length == 26 ? string.Join(" ", Enumerable.Range(0, 7).Select(i => s.Substring(i * 4, Math.Min(4, s.Length - i * 4)))) : input;
    }
    public static string Mask(string input)
    {
        var s = Normalize(input); return s.Length == 26 ? $"{s[..4]}********{s[^6..]}" : "IR********************";
    }
    public static string? BankCode(string input)
    {
        var s = Normalize(input); return s.Length >= 7 ? s.Substring(4, 3) : null;
    }
}
public static class Sayad
{

    public static bool IsValid(string? input) => Regex.IsMatch(Normalizer.Normalize(input), @"^\d{16}$");
    public static string Normalize(string input) => Normalizer.Normalize(input).Replace("-", "").Replace(" ", "");
}
public static class ChequeNumber
{
    public static bool IsValid(string? input) => Regex.IsMatch(Normalizer.Normalize(input), @"^\d{6,20}$");
    public static string Normalize(string input) => Normalizer.Normalize(input).Replace(" ", "").Replace("-", "");
}
public static class BankCatalog
{
    private static readonly Dictionary<string, string> Banks = new()
    {
        [IranBankConstants.BankCardBins.Melli] = "بانک ملی",
        [IranBankConstants.BankCardBins.MelliLegacy] = "بانک ملی",
        [IranBankConstants.BankCardBins.Mellat] = "بانک ملت",
        [IranBankConstants.BankCardBins.MellatLegacy] = "بانک ملت",
        [IranBankConstants.BankCardBins.Saderat] = "بانک صادرات",
        [IranBankConstants.BankCardBins.Tejarat] = "بانک تجارت",
        [IranBankConstants.BankCardBins.TejaratLegacy] = "بانک تجارت",
        [IranBankConstants.BankCardBins.Sepah] = "بانک سپه",
        [IranBankConstants.BankCardBins.Refah] = "بانک رفاه",
        [IranBankConstants.BankCardBins.Keshavarzi] = "بانک کشاورزی",
        [IranBankConstants.BankCardBins.KeshavarziLegacy] = "بانک کشاورزی",
        [IranBankConstants.BankCardBins.Maskan] = "بانک مسکن",
        [IranBankConstants.BankCardBins.SanatOMadan] = "بانک صنعت و معدن",
        [IranBankConstants.BankCardBins.ToseeSaderat] = "بانک توسعه صادرات",
        [IranBankConstants.BankCardBins.ToseeSaderatLegacy] = "بانک توسعه صادرات",
        [IranBankConstants.BankCardBins.Post] = "پست بانک",
        [IranBankConstants.BankCardBins.ToseeTaavon] = "بانک توسعه تعاون",
        [IranBankConstants.BankCardBins.EghtesadNovin] = "بانک اقتصاد نوین",
        [IranBankConstants.BankCardBins.Parsian] = "بانک پارسیان",
        [IranBankConstants.BankCardBins.Parsian2] = "بانک پارسیان",
        [IranBankConstants.BankCardBins.Parsian3] = "بانک پارسیان",
        [IranBankConstants.BankCardBins.Pasargad] = "بانک پاسارگاد",
        [IranBankConstants.BankCardBins.Pasargad2] = "بانک پاسارگاد",
        [IranBankConstants.BankCardBins.Karafarin] = "بانک کارآفرین",
        [IranBankConstants.BankCardBins.Karafarin2] = "بانک کارآفرین",
        [IranBankConstants.BankCardBins.Saman] = "بانک سامان",
        [IranBankConstants.BankCardBins.Sarmayeh] = "بانک سرمایه",
        [IranBankConstants.BankCardBins.Sina] = "بانک سینا",
        [IranBankConstants.BankCardBins.Shahr] = "بانک شهر",
        [IranBankConstants.BankCardBins.Shahr2] = "بانک شهر",
        [IranBankConstants.BankCardBins.Dey] = "بانک دی",
        [IranBankConstants.BankCardBins.IranZamin] = "بانک ایران زمین",
        [IranBankConstants.BankCardBins.Gardeshgari] = "بانک گردشگری",
        [IranBankConstants.BankCardBins.Gardeshgari2] = "بانک گردشگری",
        [IranBankConstants.BankCardBins.MehrIran] = "بانک مهر ایران",
        [IranBankConstants.BankCardBins.Resalat] = "بانک رسالت",
        [IranBankConstants.BankCardBins.MiddleEast] = "بانک خاورمیانه",
        [IranBankConstants.BankCardBins.Melal] = "موسسه ملل",
        [IranBankConstants.BankCardBins.Tosee] = "موسسه توسعه",
        [IranBankConstants.BankCardBins.NoorLegacy] = "موسسه نور",
        [IranBankConstants.BankCardBins.CentralBank] = "بانک مرکزی",
        [IranBankConstants.BankCardBins.AyandehLegacy] = "بانک آینده",
        [IranBankConstants.BankCardBins.AnsarLegacy] = "بانک انصار",
        [IranBankConstants.BankCardBins.HekmatIranianLegacy] = "بانک حکمت ایرانیان",
        [IranBankConstants.BankCardBins.MehrEqtesadLegacy] = "بانک مهر اقتصاد",
        [IranBankConstants.BankCardBins.GhavaminLegacy] = "بانک قوامین",
        [IranBankConstants.BankCardBins.KosarLegacy] = "موسسه کوثر"
    };
    public static bool TryGetName(string prefix, out string name) => Banks.TryGetValue(prefix, out name!);
    public static string? TryGetName(string prefix) => Banks.TryGetValue(prefix, out var n) ? n : null;
}
