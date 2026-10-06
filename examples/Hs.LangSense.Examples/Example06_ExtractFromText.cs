using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Extract structured values from free text.</summary>
public static class Example06_ExtractFromText
{
    public static void Run()
    {
        Console.WriteLine("【 06 — Extract from free text 】");
        var text = "موبایل 09121111111 ایمیل a@b.com مبلغ ۲ میلیون تومان";
        foreach (var hit in text.ExtractValues())
            Console.WriteLine($"  {hit.Type,-16} '{hit.Value}' valid={hit.IsValid} @ {hit.Start}");
        Console.WriteLine();
    }
}
