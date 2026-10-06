namespace Hs.LangSense.DateTimeSupport;

[Flags]
public enum EventType { None=0, Holiday=1, Occasion=2, National=4, Religious=8 }
public sealed record PersianEvent(PersianDateTime Date, string Title, EventType EventType);

public static class PersianEventCalendar
{
    public static IReadOnlyList<PersianEvent> GetEvents(PersianDateTime start, PersianDateTime end)
    {
        var list = new List<PersianEvent>();
        for (var y = start.Year; y <= end.Year; y++)
        {
            Add(list, y, 1, 1, "نوروز", EventType.Holiday | EventType.National);
            Add(list, y, 1, 2, "عید نوروز", EventType.Holiday | EventType.National);
            Add(list, y, 1, 3, "عید نوروز", EventType.Holiday | EventType.National);
            Add(list, y, 1, 4, "عید نوروز", EventType.Holiday | EventType.National);
            Add(list, y, 12, 29, "روز ملی شدن صنعت نفت", EventType.National);
            Add(list, y, 12, 30, "آخرین روز سال", EventType.Occasion);
        }
        return list.Where(e => e.Date >= start && e.Date <= end).OrderBy(e => e.Date).ToList();
    }
    private static void Add(List<PersianEvent> list,int y,int m,int d,string title,EventType type)
    { try { list.Add(new(new PersianDateTime(y,m,d),title,type)); } catch { } }
}

public static class PersianDateTimeEventsExtensions
{
    public static IReadOnlyList<PersianEvent> GetEventsTill(this PersianDateTime start, PersianDateTime end) => PersianEventCalendar.GetEvents(start,end);
}
