namespace Hs.LangSense;

public static class PersianNaturalDate
{
    private static readonly Dictionary<string,int> Months = new(StringComparer.OrdinalIgnoreCase)
    {
        ["فروردین"] = 1, ["اردیبهشت"] = 2, ["خرداد"] = 3, ["تیر"] = 4,
        ["مرداد"] = 5, ["شهریور"] = 6, ["مهر"] = 7, ["آبان"] = 8,
        ["آذر"] = 9, ["دی"] = 10, ["بهمن"] = 11, ["اسفند"] = 12
    };

    private static readonly Dictionary<string,int> Ordinals = new(StringComparer.OrdinalIgnoreCase)
    {
        ["اول"] = 1, ["یکم"] = 1, ["دوم"] = 2, ["دومین"] = 2, ["سوم"] = 3,
        ["سومین"] = 3, ["چهارم"] = 4, ["پنجم"] = 5, ["ششم"] = 6, ["هفتم"] = 7,
        ["هشتم"] = 8, ["نهم"] = 9, ["دهم"] = 10, ["یازدهم"] = 11, ["دوازدهم"] = 12,
        ["سیزدهم"] = 13, ["چهاردهم"] = 14, ["پانزدهم"] = 15, ["شانزدهم"] = 16,
        ["هفدهم"] = 17, ["هجدهم"] = 18, ["نوزدهم"] = 19, ["بیستم"] = 20,
        ["بیست و یکم"] = 21, ["بیست و دوم"] = 22, ["بیست و سوم"] = 23,
        ["بیست و چهارم"] = 24, ["بیست و پنجم"] = 25, ["بیست و ششم"] = 26,
        ["بیست و هفتم"] = 27, ["بیست و هشتم"] = 28, ["بیست و نهم"] = 29,
        ["سی ام"] = 30, ["سی‌ام"] = 30, ["سی و یکم"] = 31
    };

    public static bool TryParse(string? expression, DateTime? now, out DateTime date)
    {
        date = default;
        var s = Normalizer.NormalizePersian(expression).Trim();
        var baseDate = (now ?? DateTime.Now).Date;
        if (string.IsNullOrWhiteSpace(s)) return false;

        var month = Months.FirstOrDefault(x => s.Contains(x.Key, StringComparison.OrdinalIgnoreCase));
        if (month.Key is null) return false;

        var beforeMonth = s[..s.IndexOf(month.Key, StringComparison.OrdinalIgnoreCase)].Trim();
        beforeMonth = beforeMonth.Trim('،', ',', ' ');
        if (beforeMonth.EndsWith("ماه", StringComparison.OrdinalIgnoreCase)) beforeMonth = beforeMonth[..^3].Trim();

        int day;
        if (OrdinalToDay(beforeMonth, out day)) { }
        else if (int.TryParse(DigitConverter.ToEnglish(beforeMonth), out day)) { }
        else if (PersianWords.TryParse(beforeMonth.Replace(" و ", " "), out var numeric)) day = (int)numeric;
        else return false;

        var pc = new System.Globalization.PersianCalendar();
        var year = pc.GetYear(baseDate);
        try
        {
            date = pc.ToDateTime(year, month.Value, day, 0, 0, 0, 0);
            if (date < baseDate.AddDays(-1)) date = pc.ToDateTime(year + 1, month.Value, day, 0, 0, 0, 0);
            return true;
        }
        catch { return false; }
    }

    private static bool OrdinalToDay(string text, out int day)
    {
        text = text.Trim();
        if (Ordinals.TryGetValue(text, out day)) return true;
        if (text.EndsWith('م'))
        {
            var candidate = text[..^1];
            if (Ordinals.TryGetValue(candidate, out day)) return true;
        }
        day = 0; return false;
    }

    public static DateTime? Parse(string? expression, DateTime? now = null)
        => TryParse(expression, now, out var d) ? d : null;
}
