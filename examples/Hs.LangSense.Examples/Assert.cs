namespace Hs.LangSense.Examples;

/// <summary>Minimal assertion helper for the examples project (no test framework required).</summary>
public static class Check
{
    public static int Passed { get; private set; }
    public static int Failed { get; private set; }

    public static void Eq(string name, string? actual, string? expected)
    {
        if (string.Equals(actual, expected, StringComparison.Ordinal))
        {
            Passed++;
            Console.WriteLine($"  ✓ {name}");
        }
        else
        {
            Failed++;
            Console.WriteLine($"  ✗ {name}");
            Console.WriteLine($"      expected: '{expected}'");
            Console.WriteLine($"      actual  : '{actual}'");
        }
    }

    public static void True(string name, bool condition)
    {
        if (condition)
        {
            Passed++;
            Console.WriteLine($"  ✓ {name}");
        }
        else
        {
            Failed++;
            Console.WriteLine($"  ✗ {name}");
        }
    }

    public static void Summary()
    {
        Console.WriteLine();
        Console.WriteLine($"════════════════════════════════════════");
        Console.WriteLine($"  Passed: {Passed}   Failed: {Failed}");
        Console.WriteLine($"════════════════════════════════════════");
        if (Failed > 0)
            Environment.ExitCode = 1;
    }
}
