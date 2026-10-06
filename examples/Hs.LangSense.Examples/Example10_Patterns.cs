using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Build and match custom patterns without writing full regex.</summary>
public static class Example10_Patterns
{
    public static void Run()
    {
        Console.WriteLine("【 10 — Patterns 】");
        var pattern = PatternBuilder.Create("CustomerCode")
            .StartsWith("CUS-")
            .Digits(8)
            .Build();
        Console.WriteLine($"  CUS-12345678 → {pattern.IsMatch("CUS-12345678")}");
        Console.WriteLine($"  CUS-12       → {pattern.IsMatch("CUS-12")}");
        Console.WriteLine();
    }
}
