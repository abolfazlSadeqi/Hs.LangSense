using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>
/// Number ↔ words in Persian, English, Sorani and Kurmanji (and reverse).
/// </summary>
public static class Example04_NumberWords
{
    public static void Run()
    {
        Console.WriteLine("【 04 — Number ↔ words 】");

        // Number → words
        Console.WriteLine($"  1250 → fa       : {Text.NumberToWords(1250, "fa")}");
        Console.WriteLine($"  1250 → en       : {Text.NumberToWords(1250, "en")}");
        Console.WriteLine($"  1250 → sorani   : {Text.NumberToWords(1250, "sorani")}");
        Console.WriteLine($"  1250 → kurmanji : {Text.NumberToWords(1250, "kurmanji")}");
        Console.WriteLine($"  extension       : {1250L.ToWords(Language.Persian)}");

        // Words → number
        if (Text.WordsToNumber("یک هزار و دویست", out var n1))
            Console.WriteLine($"  words→num fa    : یک هزار و دویست → {n1}");
        if (Text.WordsToNumber("یەک هەزار", "sorani", out var n2))
            Console.WriteLine($"  words→num ckb   : یەک هەزار → {n2}");
        if (Text.WordsToNumber("yek hezar", "kurmanji", out var n3))
            Console.WriteLine($"  words→num kmr   : yek hezar → {n3}");

        // Words language A → words language B
        Console.WriteLine($"  fa→en           : {Text.TranslateNumberWords("یک هزار", "en")}");
        Console.WriteLine($"  en words→fa     : {Text.TranslateNumberWords("one thousand", "fa")}"); // may fail if English parse missing

        Console.WriteLine($"  cheque          : {Cheque.ToWords(12_000_000)}");
        Console.WriteLine();
    }
}
