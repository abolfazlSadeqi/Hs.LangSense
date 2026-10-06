using System.Globalization;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public enum NumberSign { Negative = -1, Zero = 0, Positive = 1 }
public enum NumberKind
{
    Unknown, Integer, Decimal, Negative, Positive, Zero, Percentage,
    Ratio, Score, Age, Year, Money, Scale
}

public sealed record NumberIntelligenceResult(
    decimal Value,
    NumberKind Kind,
    NumberSign Sign,
    bool IsInteger,
    bool IsDecimal,
    bool IsPercentage,
    bool IsRatio,
    bool IsScore,
    bool IsAge,
    bool IsYear,
    bool IsMoney,
    string Normalized);

public static class NumberAnalyzer
{
    public static NumberIntelligenceResult Analyze(string? input, ValueContext? context = null)
    {
        var normalized = Normalizer.Normalize(input);
        var sign = NumberSign.Zero;
        decimal value = 0;
        var kind = NumberKind.Unknown;

        if (PercentageParser.TryParse(normalized, out var percentage) && normalized.Contains('%'))
        {
            value = percentage; kind = NumberKind.Percentage;
        }
        else if (RatioParser.TryParse(normalized, out var ratio))
        {
            value = ratio.Value; kind = NumberKind.Ratio;
        }
        else if (Money.TryParse(normalized, out var money) && money.Currency != Currency.Unknown)
        {
            value = money.Amount; kind = NumberKind.Money;
        }
        else if (NumberParser.TryParse(normalized, out var number))
        {
            value = number; kind = decimal.Truncate(number) == number ? NumberKind.Integer : NumberKind.Decimal;
        }

        if (context == ValueContext.Percentage) kind = NumberKind.Percentage;
        else if (context is ValueContext.Score or ValueContext.StudentScore) kind = NumberKind.Score;
        else if (context == ValueContext.Age) kind = NumberKind.Age;
        else if (context is ValueContext.Year or ValueContext.PersianYear or ValueContext.GregorianYear) kind = NumberKind.Year;
        else if (context is ValueContext.Money or ValueContext.Cheque) kind = NumberKind.Money;

        sign = value < 0 ? NumberSign.Negative : value > 0 ? NumberSign.Positive : NumberSign.Zero;
        var isYear = kind == NumberKind.Year || (value >= 1_000 && value <= 9_999 && decimal.Truncate(value) == value);
        if (kind == NumberKind.Unknown) kind = decimal.Truncate(value) == value ? NumberKind.Integer : NumberKind.Decimal;

        return new(value, kind, sign, decimal.Truncate(value) == value, decimal.Truncate(value) != value,
            kind == NumberKind.Percentage, kind == NumberKind.Ratio, kind == NumberKind.Score,
            kind == NumberKind.Age, isYear, kind == NumberKind.Money, normalized);
    }
}

public static class NumberText
{
    public static bool TryParse(string? text, out decimal value, NumberSystem system = NumberSystem.Latin)
    {
        var s = text ?? string.Empty;
        s = system switch
        {
            NumberSystem.Persian => DigitConverter.ToEnglish(s),
            NumberSystem.ArabicIndic => DigitConverter.ToEnglish(s),
            _ => s
        };
        return NumberParser.TryParse(s, out value);
    }

    public static string ToWords(decimal value) => PersianWords.ToWords((long)decimal.Truncate(value));
    public static string ToWords(long value, Language language) => language switch
    {
        Language.English or Language.Latin => EnglishNumberWords.ToWords(value),
        Language.Kurmanji => KurdishWords.ToKurmanji(value),
        Language.Sorani => KurdishWords.ToSorani(value),
        _ => PersianWords.ToWords(value)
    };
}

public static class EnglishNumberWords
{
    private static readonly string[] Ones = { "zero", "one", "two", "three", "four",
        "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve",
        "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
    private static readonly string[] Tens = { "", "", "twenty", "thirty", "forty", "fifty",
        "sixty", "seventy", "eighty", "ninety" };

    public static string ToWords(long value)
    {
        if (value == 0) return "zero";
        if (value < 0) return "minus " + ToWords(-value);
        var parts = new List<string>();
        if (value >= 1_000_000_000)
        {
            parts.Add(ToWords(value / 1_000_000_000) + " billion"); value %= 1_000_000_000;
        }
        if (value >= 1_000_000)
        {
            parts.Add(ToWords(value / 1_000_000) + " million"); value %= 1_000_000;
        }
        if (value >= 1000)
        {
            parts.Add(ToWords(value / 1000) + " thousand"); value %= 1000;
        }
        if (value > 0)
            parts.Add(BelowThousand(value));
        return string.Join(" ", parts);
    }

    /// <summary>Parse simple English number words (one, thousand, …) into a decimal.</summary>
    public static bool TryParse(string? input, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var s = input.Trim().ToLowerInvariant()
            .Replace("-", " ").Replace(" and ", " ");
        var map = new Dictionary<string, long>
        {
            ["zero"] = 0,
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3,
            ["four"] = 4,
            ["five"] = 5,
            ["six"] = 6,
            ["seven"] = 7,
            ["eight"] = 8,
            ["nine"] = 9,
            ["ten"] = 10,
            ["eleven"] = 11,
            ["twelve"] = 12,
            ["thirteen"] = 13,
            ["fourteen"] = 14,
            ["fifteen"] = 15,
            ["sixteen"] = 16,
            ["seventeen"] = 17,
            ["eighteen"] = 18,
            ["nineteen"] = 19,
            ["twenty"] = 20,
            ["thirty"] = 30,
            ["forty"] = 40,
            ["fifty"] = 50,
            ["sixty"] = 60,
            ["seventy"] = 70,
            ["eighty"] = 80,
            ["ninety"] = 90,
            ["hundred"] = 100
        };
        var scale = new Dictionary<string, long>
        {
            ["thousand"] = 1000,
            ["million"] = 1_000_000,
            ["billion"] = 1_000_000_000,
            ["trillion"] = 1_000_000_000_000
        };
        long total = 0, current = 0; bool found = false;
        foreach (var token in s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (map.TryGetValue(token, out var n))
            {
                if (n == 100) { current = (current == 0 ? 1 : current) * 100; found = true; }
                else { current += n; found = true; }
            }
            else if (scale.TryGetValue(token, out var sc))
            {
                if (current == 0) current = 1;
                total += current * sc; current = 0; found = true;
            }
            else return false;
        }
        value = total + current;
        return found;
    }

    private static string BelowThousand(long n)
    {
        var p = new List<string>();
        if (n >= 100)
        {
            p.Add(Ones[(int)(n / 100)] + " hundred"); n %= 100;
        }
        if (n >= 20)
        {
            p.Add(Tens[(int)(n / 10)]); n %= 10; if (n > 0) p[^1] += "-" + Ones[(int)n];
        }
        else if (n > 0)
            p.Add(Ones[(int)n]);
        return string.Join(" ", p);
    }
}
