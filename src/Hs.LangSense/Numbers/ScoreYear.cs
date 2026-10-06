namespace Hs.LangSense;
public static class ScoreParser
{
    public static bool TryParse(string text, out decimal score, out decimal maximum)
    {
        score = 0; maximum = 20; var s = Normalizer.Normalize(text); var m = System.Text.RegularExpressions.Regex.Match(s, @"(-?\d+(?:\.\d+)?)\s*(?:از|/)\s*(\d+(?:\.\d+)?)"); if (m.Success) { score = decimal.Parse(m.Groups[1].Value); maximum = decimal.Parse(m.Groups[2].Value); return true; }
        if (decimal.TryParse(s, out score))
        {
            maximum = 20;
            return true;
        }
        var words = s.Replace("نمره", "").Trim();
        return PersianWords.TryParse(words, out score);
    }
}
public static class YearParser
{
    public static bool IsPersianYear(string value)
    {
        var s = Normalizer.Normalize(value); return int.TryParse(s, out var y) && y >= 1200 && y <= 2000;
    }
    public static bool IsGregorianYear(string value)
    {
        var s = Normalizer.Normalize(value);
        return int.TryParse(s, out var y) && y >= 1 && y <= 9999;
    }
}
