using System.Globalization;
using System.Text.RegularExpressions;
namespace Hs.LangSense;
public sealed record ParsedDate(DateTime Date,CalendarKind Calendar,string Raw);
public static class DateParser
{
    public static bool TryParse(string? input,out ParsedDate value)
    {
        value=new(DateTime.MinValue,CalendarKind.Gregorian,input??string.Empty); var s=Normalizer.Normalize(input);
        if(TryPersian(s,out var persian)){value=new(persian,CalendarKind.Persian,s);return true;}
        if(DateTime.TryParse(s,CultureInfo.InvariantCulture,DateTimeStyles.AllowWhiteSpaces,out var greg)){value=new(greg,CalendarKind.Gregorian,s);return true;}
        return false;
    }
    public static bool TryPersian(string s,out DateTime date)
    {
        date=default; var m=Regex.Match(s,@"(?<!\d)(1[34]\d{2})[/-](\d{1,2})[/-](\d{1,2})(?!\d)"); if(!m.Success)return false;
        try{date=new PersianCalendar().ToDateTime(int.Parse(m.Groups[1].Value),int.Parse(m.Groups[2].Value),int.Parse(m.Groups[3].Value),0,0,0,0);return true;}catch{return false;}
    }
    public static DateTime? Relative(string expression,DateTime? now=null)
    {
        var baseDate=(now??DateTime.Now).Date; var s=Normalizer.NormalizePersian(expression).Trim();
        if(s is "امروز" or "today")return baseDate; if(s is "فردا" or "tomorrow")return baseDate.AddDays(1); if(s is "پس فردا" or "پس‌فردا")return baseDate.AddDays(2); if(s is "دیروز" or "yesterday")return baseDate.AddDays(-1);
        if(s.Contains("هفته بعد")||s.Contains("هفته آینده"))return baseDate.AddDays(7); if(s.Contains("هفته قبل"))return baseDate.AddDays(-7); if(s.Contains("ماه آینده"))return baseDate.AddMonths(1); if(s.Contains("ماه قبل"))return baseDate.AddMonths(-1);
        return PersianNaturalDate.TryParse(s,baseDate,out var natural)?natural:null;
    }
}
public static class PersianDate { public static string Format(DateTime date){var c=new PersianCalendar();return $"{c.GetYear(date):0000}/{c.GetMonth(date):00}/{c.GetDayOfMonth(date):00}";} public static DateTime From(int year,int month,int day)=>new PersianCalendar().ToDateTime(year,month,day,0,0,0,0); public static int Year(DateTime date)=>new PersianCalendar().GetYear(date);public static int Month(DateTime date)=>new PersianCalendar().GetMonth(date);public static int Day(DateTime date)=>new PersianCalendar().GetDayOfMonth(date); }
