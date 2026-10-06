using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>
/// Normalize dirty text. Use Text.Normalize / .NormalizeText() — NEVER string.Normalize() (BCL Unicode).
/// </summary>
public static class Example05_NormalizeAndAnalyze
{
    public static void Run()
    {
        Console.WriteLine("【 05 — Normalize & Analyze 】");

        // Arabic kaf U+0643, ZWNJ U+200C, Persian digits
        var dirty = "  \u0643\u062f  \u200c  \u06f1\u06f2\u06f3  ";
        var viaText = Text.Normalize(dirty);
        var viaExt = dirty.NormalizeText();

        Console.WriteLine($"  dirty              : '{dirty}'");
        Console.WriteLine($"  Text.Normalize     : '{viaText}'");
        Console.WriteLine($"  .NormalizeText()   : '{viaExt}'");
        Console.WriteLine($"  BCL string.Normalize (WRONG API): '{dirty.Normalize()}'");

        Check.Eq("ك → ک + digits Latin + trim", viaText, "کد 123");
        Check.Eq("NormalizeText same as Text.Normalize", viaExt, viaText);
        Check.Eq("علي → علی", Text.Normalize("علي"), "علی");
        Check.Eq("ي → ی", Text.Normalize("ي"), "ی");
        Check.Eq("ك → ک", Text.Normalize("ك"), "ک");

        var result = " ۰۹۱۲-۱۱۱-۱۱۱۱ ".Analyze();
        Console.WriteLine($"  kind       : {result.Type}");
        Console.WriteLine($"  normalized : {result.NormalizedValue}");
        Console.WriteLine($"  valid      : {result.IsValid}");
        Check.True("mobile analyze valid", result.IsValid);
        Console.WriteLine();
    }
}
