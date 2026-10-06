using System.Globalization;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public sealed record ParsedTime(TimeSpan Time, string Raw);
public sealed record ParsedDateTime(DateTime Value, CalendarKind Calendar, string Raw);

public static class TimeParser
{
    public static bool TryParse(string? input, out TimeSpan value)
    {
        value = default;
        var s = Normalizer.Normalize(input).Trim();
        if (TimeSpan.TryParseExact(s, new[] { @"h\:mm", @"hh\:mm", @"h\:mm\:ss", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out value)) return true;
        var m = Regex.Match(s, @"^(?<h>\d{1,2})[:٫](?<m>\d{2})(?:[:](?<s>\d{2}))?$");
        if (!m.Success) return false;
        var h = int.Parse(m.Groups["h"].Value); var min = int.Parse(m.Groups["m"].Value); var sec = m.Groups["s"].Success ? int.Parse(m.Groups["s"].Value) : 0;
        if (h > 23 || min > 59 || sec > 59) return false;
        value = new TimeSpan(h, min, sec); return true;
    }
}

public static class DateTimeParser
{
    public static bool TryParse(string? input, out ParsedDateTime value)
    {
        value = new ParsedDateTime(default, CalendarKind.Gregorian, input ?? string.Empty);
        var s = Normalizer.Normalize(input).Trim();
        if (!s.Contains(":", StringComparison.Ordinal)) return false;
        if (DateParser.TryPersian(s, out var persian)) { value = new(persian, CalendarKind.Persian, s); return true; }
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var gregorian)) { value = new(gregorian, CalendarKind.Gregorian, s); return true; }
        return false;
    }
}

public static class DateParts
{
    public static int? Year(string? input) => DateParser.TryParse(input, out var d) ? d.Calendar == CalendarKind.Persian ? PersianDate.Year(d.Date) : d.Date.Year : null;
    public static int? Month(string? input) => DateParser.TryParse(input, out var d) ? d.Calendar == CalendarKind.Persian ? PersianDate.Month(d.Date) : d.Date.Month : null;
    public static int? Day(string? input) => DateParser.TryParse(input, out var d) ? d.Calendar == CalendarKind.Persian ? PersianDate.Day(d.Date) : d.Date.Day : null;
}
