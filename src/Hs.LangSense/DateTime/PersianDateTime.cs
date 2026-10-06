using System.Globalization;
using System.Text.RegularExpressions;

namespace Hs.LangSense.DateTimeSupport;

/// <summary>Immutable Jalali date/time value with Gregorian conversion, parsing and arithmetic.</summary>
public readonly struct PersianDateTime : IComparable<PersianDateTime>, IEquatable<PersianDateTime>, IFormattable
{
    private static readonly PersianCalendar Calendar = new();
    private readonly System.DateTime _value;

    public PersianDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 0, 0) { }
    public PersianDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0) { }
    public PersianDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
    {
        _value = Calendar.ToDateTime(year, month, day, hour, minute, second, millisecond);
    }
    public PersianDateTime(System.DateTime gregorian) => _value = gregorian;

    public int Year => Calendar.GetYear(_value);
    public int Month => Calendar.GetMonth(_value);
    public int Day => Calendar.GetDayOfMonth(_value);
    public int Hour => _value.Hour;
    public int Minute => _value.Minute;
    public int Second => _value.Second;
    public int Millisecond => _value.Millisecond;
    public DayOfWeek DayOfWeek => _value.DayOfWeek;
    public bool IsLeapYear => Calendar.IsLeapYear(Year);
    public int DaysInMonth => Calendar.GetDaysInMonth(Year, Month);
    public System.DateTime GregorianDateTime => _value;
    public static PersianDateTime Now => new(System.DateTime.Now);
    public static PersianDateTime Today => new(System.DateTime.Today);

    public PersianDateTime AddDays(double value) => new(_value.AddDays(value));
    public PersianDateTime AddHours(double value) => new(_value.AddHours(value));
    public PersianDateTime AddMinutes(double value) => new(_value.AddMinutes(value));
    public PersianDateTime AddSeconds(double value) => new(_value.AddSeconds(value));
    public PersianDateTime AddMilliseconds(double value) => new(_value.AddMilliseconds(value));
    public PersianDateTime AddMonths(int value) => new(_value.AddMonths(value));
    public PersianDateTime AddYears(int value) => new(_value.AddYears(value));
    public PersianDateTime Date => new(_value.Date);

    public static PersianDateTime Parse(string input)
    {
        if (!TryParse(input, out var value)) throw new FormatException($"Invalid Persian date/time: {input}");
        return value;
    }
    public static bool TryParse(string? input, out PersianDateTime value)
    {
        value = default;
        var s = Normalizer.Normalize(input);
        if (string.IsNullOrWhiteSpace(s)) return false;
        s = DigitConverter.ToEnglish(s).Trim();
        var parts = s.Split(new[] { '/', '-', '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        try
        {
            if (parts.Length is >= 3 and <= 6 && parts.All(p => int.TryParse(p, out _)))
            {
                var y = int.Parse(parts[0]);
                if (y is >= 1200 and <= 1600)
                {
                    var m = int.Parse(parts[1]); var d = int.Parse(parts[2]);
                    var h = parts.Length > 3 ? int.Parse(parts[3]) : 0;
                    var min = parts.Length > 4 ? int.Parse(parts[4]) : 0;
                    var sec = parts.Length > 5 ? int.Parse(parts[5]) : 0;
                    value = new PersianDateTime(y, m, d, h, min, sec); return true;
                }
            }
            if (Regex.IsMatch(s, @"^\d{8}(\d{6})?$"))
            {
                var y = int.Parse(s[..4]); var m = int.Parse(s.Substring(4, 2)); var d = int.Parse(s.Substring(6, 2));
                var h = s.Length >= 14 ? int.Parse(s.Substring(8, 2)) : 0;
                var min = s.Length >= 14 ? int.Parse(s.Substring(10, 2)) : 0;
                var sec = s.Length >= 14 ? int.Parse(s.Substring(12, 2)) : 0;
                if (y is >= 1200 and <= 1600) { value = new PersianDateTime(y, m, d, h, min, sec); return true; }
            }
            if (long.TryParse(s, out var epoch))
            {
                if (s.Length == 10) { value = new PersianDateTime(System.DateTimeOffset.FromUnixTimeSeconds(epoch).LocalDateTime); return true; }
                if (s.Length == 13) { value = new PersianDateTime(System.DateTimeOffset.FromUnixTimeMilliseconds(epoch).LocalDateTime); return true; }
            }
        }
        catch (ArgumentOutOfRangeException) { }
        return false;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        format ??= "yyyy/MM/dd HH:mm:ss";
        return format.Replace("yyyy", Year.ToString("0000", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("MM", Month.ToString("00", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("dd", Day.ToString("00", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("HH", Hour.ToString("00", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("mm", Minute.ToString("00", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("ss", Second.ToString("00", CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
    public override string ToString() => ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
    public int CompareTo(PersianDateTime other) => _value.CompareTo(other._value);
    public bool Equals(PersianDateTime other) => _value == other._value;
    public override bool Equals(object? obj) => obj is PersianDateTime other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public static implicit operator System.DateTime(PersianDateTime value) => value._value;
    public static implicit operator PersianDateTime(System.DateTime value) => new(value);
    public static PersianDateTime operator +(PersianDateTime value, TimeSpan span) => new(value._value + span);
    public static PersianDateTime operator -(PersianDateTime value, TimeSpan span) => new(value._value - span);
    public static TimeSpan operator -(PersianDateTime left, PersianDateTime right) => left._value - right._value;
    public static bool operator <(PersianDateTime a, PersianDateTime b) => a._value < b._value;
    public static bool operator >(PersianDateTime a, PersianDateTime b) => a._value > b._value;
    public static bool operator <=(PersianDateTime a, PersianDateTime b) => a._value <= b._value;
    public static bool operator >=(PersianDateTime a, PersianDateTime b) => a._value >= b._value;
    public static bool operator ==(PersianDateTime a, PersianDateTime b) => a._value == b._value;
    public static bool operator !=(PersianDateTime a, PersianDateTime b) => a._value != b._value;
}
