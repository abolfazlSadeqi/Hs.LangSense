using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>
/// Kurdish Sorani and Kurmanji: conversion, digits, keyboard, number-words.
/// Note: ConvertTo(kurmanji) is letter transliteration, not semantic translation.
/// </summary>
public static class Example13_Kurdish
{
    public static void Run()
    {
        Console.WriteLine("【 13 — Kurdish Sorani & Kurmanji 】");

        Console.WriteLine($"  ConvertTo sorani   : {"١٢٣ سڵاو".ConvertTo("sorani")}");
        // Kurmanji path = transliteration to Latin (deterministic), not full orthography conversion
        Console.WriteLine($"  ConvertTo kurmanji : {"سڵاو".ConvertTo("kurmanji")}");
        Console.WriteLine($"  ToSorani()         : {"١٢٣ سڵاو".ToSorani()}");

        Console.WriteLine($"  digits→Persian     : {"12345".ToPersianDigits()}");
        Console.WriteLine($"  digits→Latin       : {"۱۲۳۴۵".ToLatinDigits()}");

        Console.WriteLine($"  keyboard Sorani    : {Text.Keyboard("slaw", KeyboardLayout.English, KeyboardLayout.Sorani)}");
        Console.WriteLine($"  FixSoraniKeyboard  : {"hello".FixSoraniKeyboard()}");

        Console.WriteLine($"  1250 Sorani        : {KurdishWords.ToSorani(1250)}");
        Console.WriteLine($"  1250 Kurmanji      : {KurdishWords.ToKurmanji(1250)}");
        Console.WriteLine($"  ToWords(Sorani)    : {NumberText.ToWords(42, Language.Sorani)}");
        Console.WriteLine($"  ToWords(Kurmanji)  : {NumberText.ToWords(42, Language.Kurmanji)}");

        if (KurdishWords.TryParseSorani("یەک هەزار", out var sn))
            Console.WriteLine($"  parse Sorani       : یەک هەزار → {sn}");
        if (KurdishWords.TryParseKurmanji("yek hezar", out var kn))
            Console.WriteLine($"  parse Kurmanji     : yek hezar → {kn}");

        // Full word هەڤاڵ (friend), not the three isolated letters ێەڵ
        Console.WriteLine($"  translit هەڤاڵ     : {Text.Transliterate("هەڤاڵ")}");
        Console.WriteLine();
    }
}
