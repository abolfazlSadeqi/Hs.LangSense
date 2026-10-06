using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Convert digits between Latin, Persian and Arabic-Indic systems.</summary>
public static class Example02_Digits
{
    public static void Run()
    {
        Console.WriteLine("【 02 — Digits 】");
        Console.WriteLine($"  Arabic → Persian : {Text.Digits("١٢٣٤٥", NumberSystem.Persian)}");
        Console.WriteLine($"  Persian → Latin  : {"۱۲۳۴۵".ToLatinDigits()}");
        Console.WriteLine($"  Latin → Arabic   : {"12345".ToArabicDigits()}");
        Console.WriteLine($"  Latin → Persian  : {"12345".ToPersianDigits()}");
        Console.WriteLine($"  ToDigits(Persian): {"123".ToDigits(NumberSystem.Persian)}");
        Console.WriteLine();
    }
}
