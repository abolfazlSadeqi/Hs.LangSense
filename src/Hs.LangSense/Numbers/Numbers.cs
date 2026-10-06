using System.Globalization;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public static class NumberParser
{
    private static readonly Dictionary<string, decimal> Units = new(StringComparer.OrdinalIgnoreCase)
    {
        ["هزار"] = 1_000m, ["کیلو"] = 1_000m, ["k"] = 1_000m, ["thousand"] = 1_000m,
        ["میلیون"] = 1_000_000m, ["مليون"] = 1_000_000m, ["m"] = 1_000_000m, ["million"] = 1_000_000m,
        ["میلیارد"] = 1_000_000_000m, ["مليار"] = 1_000_000_000m, ["b"] = 1_000_000_000m, ["billion"] = 1_000_000_000m,
        ["تریلیون"] = 1_000_000_000_000m, ["trillion"] = 1_000_000_000_000m
    };

    public static bool TryParse(string? input, out decimal value)
    {
        value = 0;
        var s = Normalizer.Normalize(input).Trim();
        if (string.IsNullOrWhiteSpace(s)) return false;
        s = Regex.Replace(s, @"\s+", " ");
        s = s.Replace(",", string.Empty).Replace("٬", string.Empty);

        if (decimal.TryParse(s, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value)) return true;
        if (TryParseScaled(s, out value)) return true;
        if (PersianWords.TryParse(s, out value)) return true;
        if (EnglishWords.TryParse(s, out value)) return true;
        if (ArabicWords.TryParse(s, out value)) return true;
        return false;
    }

    private static bool TryParseScaled(string s, out decimal value)
    {
        value = 0;
        var match = Regex.Match(s, @"^(?<n>[+-]?[0-9]+(?:\.[0-9]+)?)\s*(?<scale>هزار|کیلو|میلیون|مليون|میلیارد|مليار|تریلیون|thousand|million|billion|trillion|k|m|b)(?:\s+(?:تومان|ریال))?$", RegexOptions.IgnoreCase);
        if (!match.Success) return false;
        if (!decimal.TryParse(match.Groups["n"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var n)) return false;
        return Units.TryGetValue(match.Groups["scale"].Value, out var scale) && ((value = n * scale) >= decimal.MinValue);
    }
}

public static class PersianWords
{
    private static readonly Dictionary<string, int> Small = new(StringComparer.Ordinal)
    {
        ["صفر"] = 0,["یک"] = 1,["اول"] = 1,["دو"] = 2,["دوم"] = 2,["سه"] = 3,["چهار"] = 4,["پنج"] = 5,["شش"] = 6,["هفت"] = 7,["هشت"] = 8,["نه"] = 9,
        ["ده"] = 10,["یازده"] = 11,["دوازده"] = 12,["سیزده"] = 13,["چهارده"] = 14,["پانزده"] = 15,["شانزده"] = 16,["هفده"] = 17,["هجده"] = 18,["نوزده"] = 19,
        ["بیست"] = 20,["سی"] = 30,["چهل"] = 40,["پنجاه"] = 50,["شصت"] = 60,["هفتاد"] = 70,["هشتاد"] = 80,["نود"] = 90,
        ["صد"] = 100,["یکصد"] = 100,["دویست"] = 200,["سیصد"] = 300,["چهارصد"] = 400,["پانصد"] = 500,["ششصد"] = 600,["هفتصد"] = 700,["هشتصد"] = 800,["نهصد"] = 900
    };
    private static readonly Dictionary<string,long> Scale = new(StringComparer.Ordinal)
    { ["هزار"] = 1000, ["میلیون"] = 1_000_000, ["میلیارد"] = 1_000_000_000, ["تریلیون"] = 1_000_000_000_000 };

    public static bool TryParse(string? input, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var s = input.Trim();
        // Only treat " و " as a connector between number parts — never strip و inside words (دویست، نوزده، …)
        s = s.Replace("منفی ", "-", StringComparison.Ordinal);
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+و\s+", " ");
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim();

        var negative = s.StartsWith('-');
        if (negative) s = s[1..].Trim();

        long total = 0, current = 0;
        var found = false;

        foreach (var token in s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (Small.TryGetValue(token, out var n))
            {
                current += n;
                found = true;
                continue;
            }
            if (Scale.TryGetValue(token, out var scale))
            {
                // "هزار" alone means 1000
                if (current == 0) current = 1;
                total += current * scale;
                current = 0;
                found = true;
                continue;
            }
            return false;
        }

        value = total + current;
        if (negative) value = -value;
        return found;
    }

    public static string ToWords(long value)
    {
        if (value == 0) return "صفر";
        if (value < 0) return "منفی " + ToWords(-value);
        var parts = new List<string>();
        foreach (var (scale, name) in new[] { (1_000_000_000_000L,"تریلیون"),(1_000_000_000L,"میلیارد"),(1_000_000L,"میلیون"),(1000L,"هزار") })
        {
            if (value >= scale) { parts.Add(ToBelow1000(value / scale) + " " + name); value %= scale; }
        }
        if (value > 0) parts.Add(ToBelow1000(value));
        return string.Join(" و ", parts);
    }

    private static string ToBelow1000(long n)
    {
        var hundreds = new[] { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        var ones = new[] { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        var teens = new[] { "","یک","دو","سه","چهار","پنج","شش","هفت","هشت","نه","ده","یازده","دوازده","سیزده","چهارده","پانزده","شانزده","هفده","هجده","نوزده" };
        var tens = new[] { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        var p = new List<string>();
        if (n >= 100) { p.Add(hundreds[(int)(n / 100)]); n %= 100; }
        if (n >= 20) { p.Add(tens[(int)(n / 10)]); n %= 10; if (n > 0) p.Add(ones[(int)n]); }
        else if (n > 0) p.Add(teens[(int)n]);
        return string.Join(" و ", p);
    }
}

public static class EnglishWords
{
    public static bool TryParse(string? input, out decimal value)
    {
        value = 0; var s = (input ?? string.Empty).ToLowerInvariant().Replace('-', ' ');
        var d = new Dictionary<string,int>(StringComparer.Ordinal) { ["zero"]=0,["one"]=1,["two"]=2,["three"]=3,["four"]=4,["five"]=5,["six"]=6,["seven"]=7,["eight"]=8,["nine"]=9,["ten"]=10,["eleven"]=11,["twelve"]=12,["thirteen"]=13,["fourteen"]=14,["fifteen"]=15,["sixteen"]=16,["seventeen"]=17,["eighteen"]=18,["nineteen"]=19,["twenty"]=20,["thirty"]=30,["forty"]=40,["fifty"]=50,["sixty"]=60,["seventy"]=70,["eighty"]=80,["ninety"]=90 };
        var sc = new Dictionary<string,long> { ["thousand"]=1000,["million"]=1_000_000,["billion"]=1_000_000_000,["trillion"]=1_000_000_000_000 };
        long total=0,current=0; var ok=false;
        foreach(var t in s.Split(' ',StringSplitOptions.RemoveEmptyEntries))
        { if(t=="and") continue; if(d.TryGetValue(t,out var n)){current+=n;ok=true;} else if(t=="hundred"){current*=100;ok=true;} else if(sc.TryGetValue(t,out var x)){total+=current*x;current=0;ok=true;} else return false; }
        value=total+current; return ok;
    }
}

public static class ArabicWords
{
    public static bool TryParse(string? input, out decimal value)
    {
        var map = new Dictionary<string,string> { ["صفر"]="صفر",["واحد"]="یک",["واحدة"]="یک",["اثنان"]="دو",["اثنين"]="دو",["ثلاثة"]="سه",["أربعة"]="چهار",["خمسة"]="پنج",["ستة"]="شش",["سبعة"]="هفت",["ثمانية"]="هشت",["تسعة"]="نه",["عشرة"]="ده",["مئة"]="صد",["مائة"]="صد",["ألف"]="هزار",["الف"]="هزار",["مليون"]="میلیون",["مليار"]="میلیارد" };
        var s=input ?? string.Empty; foreach(var kv in map) s=s.Replace(kv.Key,kv.Value,StringComparison.Ordinal);
        return PersianWords.TryParse(s,out value);
    }
}

public enum Currency { Unknown, Rial, Toman }
public sealed record MoneyValue(decimal Amount, Currency Currency, string? Raw = null)
{
    public decimal Rial => Currency == Currency.Toman ? Amount * 10 : Amount;
    public decimal Toman => Currency == Currency.Rial ? Amount / 10 : Amount;
    public string Words => PersianWords.ToWords((long)Amount);
    public string ToWords() => Words;
    public string Format() => Money.Format(Amount, Currency);
    public string Format(int? decimalPlaces) => decimalPlaces.HasValue ? $"{Amount.ToString($"N{decimalPlaces.Value}", System.Globalization.CultureInfo.InvariantCulture)} {(Currency == Currency.Rial ? "ریال" : "تومان")}" : Format();
}

public static class Money
{
    public static bool TryParse(string? input, out MoneyValue value)
    {
        value = new(0, Currency.Unknown, input);
        var s = Normalizer.Normalize(input).Trim();
        if (!NumberParser.TryParse(s, out var amount)) return false;
        var currency = s.Contains("تومان", StringComparison.OrdinalIgnoreCase) ? Currency.Toman : s.Contains("ریال", StringComparison.OrdinalIgnoreCase) ? Currency.Rial : Currency.Unknown;
        value = new(amount, currency, input); return true;
    }
    public static decimal ToRial(decimal amount, Currency currency) => currency == Currency.Toman ? amount * 10 : amount;
    public static decimal ToToman(decimal amount, Currency currency) => currency == Currency.Rial ? amount / 10 : amount;
    public static decimal Convert(decimal amount, Currency from, Currency to) => from == to || from == Currency.Unknown || to == Currency.Unknown ? amount : from == Currency.Toman ? amount * 10 : amount / 10;
    public static decimal Round(decimal amount, int decimals = 0, MidpointRounding mode = MidpointRounding.ToEven) => Math.Round(amount, decimals, mode);
    public static string Format(decimal amount, Currency currency = Currency.Toman) => $"{amount:N0} {(currency == Currency.Rial ? "ریال" : "تومان")}";
    public static string ToWords(decimal amount) => PersianWords.ToWords((long)decimal.Truncate(amount));
}

public static class PercentageParser
{
    public static bool TryParse(string? input, out decimal value)
    {
        var s = Normalizer.Normalize(input).Trim().Replace("درصد", string.Empty).Replace("%", string.Empty).Trim();
        return NumberParser.TryParse(s, out value);
    }
    public static decimal ToFraction(decimal percentage) => percentage / 100m;
    public static decimal FromFraction(decimal fraction) => fraction * 100m;
}
