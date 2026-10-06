using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Quick reference of the most useful string extensions.</summary>
public static class Example12_ExtensionsQuickReference
{
    public static void Run()
    {
        Console.WriteLine("【 12 — Extensions quick reference 】");
        var s = "علي ۱۲۳";
        Console.WriteLine($"  ToPersian        : {s.ToPersian()}");
        Console.WriteLine($"  ToLatinDigits    : {s.ToLatinDigits()}");
        Console.WriteLine($"  NormalizeText    : {s.NormalizeText()}");
        Console.WriteLine($"  Transliterate    : {"علی".Transliterate()}");
        Console.WriteLine($"  IsMobile         : {"09121111111".IsMobile()}");
        Console.WriteLine($"  SameAs           : {"علي".SameAs("علی")}");
        Console.WriteLine($"  MaskSensitive    : {"09121111111".MaskSensitive()}");
        Console.WriteLine();
    }
}
