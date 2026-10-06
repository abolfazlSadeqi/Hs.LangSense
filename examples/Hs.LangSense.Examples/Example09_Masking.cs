using Hs.LangSense;

namespace Hs.LangSense.Examples;

public static class Example09_Masking
{
    public static void Run()
    {
        Console.WriteLine("【 09 — Masking 】");
        var original = "کارت 6037991111111111 موبایل 09121111111";
        var masked = SensitiveData.Mask(original);
        var redacted = SensitiveData.Redact(original);

        Console.WriteLine($"  original : {original}");
        Console.WriteLine($"  masked   : {masked}");
        Console.WriteLine($"  redacted : {redacted}");

        Check.True("redact keeps space before موبایل", redacted.Contains("[REDACTED] موبایل") || redacted.Contains("[REDACTED]  موبایل"));
        Check.True("mask keeps space", masked.Contains("5670 موبایل") || masked.Contains("5670  موبایل"));
        Console.WriteLine();
    }
}
