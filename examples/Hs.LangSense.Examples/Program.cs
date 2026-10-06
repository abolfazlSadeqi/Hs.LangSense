using System.Text;
using Hs.LangSense.Examples;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("════════════════════════════════════════");
Console.WriteLine("  Hs.LangSense — Examples + Assertions");
Console.WriteLine("════════════════════════════════════════");
Console.WriteLine();

Example01_ConvertToLanguage.Run();
Example02_Digits.Run();
Example03_Keyboard.Run();
Example04_NumberWords.Run();
Example05_NormalizeAndAnalyze.Run();
Example06_ExtractFromText.Run();
Example07_SearchAndCompare.Run();
Example08_IranValidation.Run();
Example09_Masking.Run();
Example10_Patterns.Run();
Example11_DatesAndMoney.Run();
Example12_ExtensionsQuickReference.Run();
Example13_Kurdish.Run();

Check.Summary();
