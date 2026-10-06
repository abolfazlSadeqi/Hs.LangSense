namespace Hs.LangSense.TextAnalysis;

public sealed record PersianTextQualityReport(
    int Length,
    int PersianLetters,
    int ArabicLetters,
    int LatinLetters,
    int Digits,
    int InvisibleCharacters,
    int TatweelCharacters,
    int MultipleSpaces,
    int SuspiciousCharacters,
    double PersianRatio,
    double QualityScore,
    IReadOnlyList<string> Suggestions);

public static class PersianTextQuality
{
    public static PersianTextQualityReport Analyze(string? text)
    {
        var s = text ?? string.Empty;
        var p = s.Count(PersianText.IsPersianLetter);
        var a = s.Count(PersianText.IsArabicLetter);
        var latin = s.Count(c => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z');
        var digits = s.Count(char.IsDigit);
        var invisible = InvisibleCharacterAnalyzer.Find(s).Count;
        var tatweel = s.Count(c => c == '\u0640');
        var spaces = System.Text.RegularExpressions.Regex.Matches(s, @" {2,}").Count;
        var suspicious = s.Count(c => c is '\uFFFD' or '\u061C');
        var letters = Math.Max(1, p + a + latin);
        var issues = new List<string>();
        if (a > 0) issues.Add("Arabic/Persian letter variants should be normalized.");
        if (invisible > 0) issues.Add("Invisible/control characters were detected.");
        if (tatweel > 0) issues.Add("Tatweel characters were detected.");
        if (spaces > 0) issues.Add("Repeated spaces were detected.");
        if (suspicious > 0) issues.Add("Suspicious replacement/directional characters were detected.");
        var penalty = Math.Min(1, (a + invisible + tatweel + spaces + suspicious) / (double)Math.Max(1, s.Length));
        var score = Math.Max(0, 100 * (1 - penalty));
        return new(s.Length, p, a, latin, digits, invisible, tatweel, spaces, suspicious, p / (double)letters, score, issues);
    }
}
