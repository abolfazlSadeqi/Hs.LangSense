namespace Hs.LangSense;
public static class NumberSystemConverter { public static string Convert(string value,NumberSystem from,NumberSystem to){var latin=DigitConverter.ToEnglish(value);return DigitConverter.FromSystem(latin,to);} }
