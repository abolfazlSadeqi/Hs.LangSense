using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Fix text typed with the wrong keyboard layout (English keys as Persian).</summary>
public static class Example03_Keyboard
{
    public static void Run()
    {
        Console.WriteLine("【 03 — Keyboard fix 】");
        Console.WriteLine($"  jsj  → {"jsj".FixPersianKeyboard()}");
        Console.WriteLine($"  sghl → {"sghl".FixPersianKeyboard()}");
        Console.WriteLine($"  generic API: {Text.Keyboard("jsj", KeyboardLayout.English, KeyboardLayout.Persian)}");
        Console.WriteLine();
    }
}
