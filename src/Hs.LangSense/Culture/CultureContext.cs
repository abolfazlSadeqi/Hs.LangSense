namespace Hs.LangSense.Culture;

public sealed record ValueCulture(string Name, string DecimalSeparator, string GroupSeparator, string CurrencySymbol, string CurrencyName);

public static class ValueCultures
{
    public static ValueCulture PersianIran { get; } = new("fa-IR", "٫", "٬", "﷼", "ریال");
    public static ValueCulture Arabic { get; } = new("ar", "٫", "٬", "ر.س", "ريال");
    public static ValueCulture English { get; } = new("en-US", ".", ",", "$", "Dollar");
}

public static class CultureResolver
{
    public static ValueCulture Resolve(string? culture) => culture?.ToLowerInvariant() switch
    {
        "fa" or "fa-ir" => ValueCultures.PersianIran,
        "ar" or "ar-sa" => ValueCultures.Arabic,
        _ => ValueCultures.English
    };
}
