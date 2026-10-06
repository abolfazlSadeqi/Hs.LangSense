using Hs.LangSense;

namespace Hs.LangSense.Examples;

public static class Example01_ConvertToLanguage
{
    public static void Run()
    {
        Console.WriteLine("【 01 — Convert to language 】");
        var mixed = "١٢٣ علي و ۱۲۳";

        var fa = Text.ToPersian(mixed);
        var ar = Text.ToArabic(mixed);
        var en = Text.ToEnglish(mixed); // transliteration + Latin digits

        Console.WriteLine($"  input     : {mixed}");
        Console.WriteLine($"  ToPersian : {fa}");
        Console.WriteLine($"  ToArabic  : {ar}");
        Console.WriteLine($"  ToEnglish : {en}  (transliteration, not translation)");

        Check.Eq("ToPersian digits+yeh", fa, "۱۲۳ علی و ۱۲۳");
        Check.True("ToEnglish contains 123", en.Contains("123"));
        Check.True("ToEnglish latin only digits", !en.Any(c => c is >= '\u06F0' and <= '\u06F9' or >= '\u0660' and <= '\u0669'));
        Console.WriteLine();
    }
}
