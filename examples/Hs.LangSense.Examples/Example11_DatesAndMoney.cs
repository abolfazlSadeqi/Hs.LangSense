using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Parse dates and money from multilingual text.</summary>
public static class Example11_DatesAndMoney
{
    public static void Run()
    {
        Console.WriteLine("【 11 — Dates & money 】");
        var dt = "فردا".ToDate(DateTime.Today);
        Console.WriteLine($"  'فردا'.ToDate() → {dt:yyyy-MM-dd}");

        if ("۲.۵ میلیون تومان".TryMoney(out var money))
            Console.WriteLine($"  money → amount={money.Amount} currency={money.Currency}");

        if ("۱۲۳۴۵".TryNumber(out var n))
            Console.WriteLine($"  number → {n}");
        Console.WriteLine();
    }
}
