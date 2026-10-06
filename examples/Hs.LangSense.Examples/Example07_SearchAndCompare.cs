using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Semantic equality and search keys (ی/ي ، ک/ك ، digits).</summary>
public static class Example07_SearchAndCompare
{
    public static void Run()
    {
        Console.WriteLine("【 07 — Search & compare 】");
        Console.WriteLine($"  علي SameAs علی     : {"علي".SameAs("علی")}");
        Console.WriteLine($"  ١٢٣ SameAs 123     : {"١٢٣".SameAs("123")}");
        Console.WriteLine($"  fingerprint علی    : {"علی".Fingerprint()[..16]}...");
        Console.WriteLine($"  search key         : {"علی رضا".ToSearchKey()[..16]}...");
        Console.WriteLine();
    }
}
