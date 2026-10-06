using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

/// <summary>
/// Converts digits between Latin (0-9), Persian (۰-۹) and Arabic-Indic (٠-٩) systems.
/// All conversions go through Latin so mixed input is handled correctly.
/// </summary>
/// <summary>
/// Converts digits between Latin (0-9), Persian (۰-۹) and Arabic-Indic (٠-٩).
/// Every conversion normalizes through Latin first so mixed input is always handled.
/// </summary>
public static class DigitConverter
{
    private const string Latin = "0123456789";
    private const string Persian = "۰۱۲۳۴۵۶۷۸۹";
    private const string Arabic = "٠١٢٣٤٥٦٧٨٩";

    /// <summary>Any digit system → Latin 0-9.</summary>
    public static string ToLatin(string? s) => ToEnglish(s);

    /// <summary>Any digit system → Latin 0-9.</summary>
    public static string ToEnglish(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s ?? "";
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            int d = DigitValue(c);
            sb.Append(d >= 0 ? Latin[d] : c);
        }
        return sb.ToString();
    }

    public static string ToEnglishPersianDigits(string? s) => ToEnglish(s);
    public static string ToEnglishArabicIndicDigits(string? s) => ToEnglish(s);

    /// <summary>Any digit system → Persian ۰-۹.</summary>
    public static string ToPersian(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s ?? "";
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            int d = DigitValue(c);
            sb.Append(d >= 0 ? Persian[d] : c);
        }
        return sb.ToString();
    }

    /// <summary>Any digit system → Arabic-Indic ٠-٩.</summary>
    public static string ToArabicIndic(string? s)
    {
        if (string.IsNullOrEmpty(s)) return s ?? "";
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            int d = DigitValue(c);
            sb.Append(d >= 0 ? Arabic[d] : c);
        }
        return sb.ToString();
    }

    /// <summary>Convert digits to the requested number system.</summary>
    public static string FromSystem(string? s, NumberSystem system) => system switch
    {
        NumberSystem.Persian => ToPersian(s),
        NumberSystem.ArabicIndic => ToArabicIndic(s),
        _ => ToEnglish(s)
    };

    /// <summary>Returns 0-9 for any digit character, or -1 if not a digit.</summary>
    public static int DigitValue(char c)
    {
        if (c is >= '0' and <= '9') return c - '0';
        if (c is >= '۰' and <= '۹') return c - '۰';
        if (c is >= '٠' and <= '٩') return c - '٠';
        return -1;
    }
}

public enum KeyboardLayout { English, Persian, Arabic, Sorani, Custom }
public sealed class KeyboardLayoutDefinition { public string Name { get; init; }=""; public IReadOnlyDictionary<char,char> Map { get; init; }=new Dictionary<char,char>(); }
public static class KeyboardLayouts
{
    public static readonly KeyboardLayoutDefinition EnglishToPersian = new()
    {
        Name="English→Persian",
        Map=new Dictionary<char,char>
        {
            ['q']='ض',['w']='ص',['e']='ث',['r']='ق',['t']='ف',['y']='غ',['u']='ع',['i']='ه',['o']='خ',['p']='ح',
            ['[']='ج',[']']='چ',['a']='ش',['s']='س',['d']='ی',['f']='ب',['g']='ل',['h']='ا',['j']='ت',['k']='ن',['l']='م',
            [';']='ک', ['z']='ظ',['x']='ط',['c']='ز',['v']='ر',['b']='ذ',['n']='د',['m']='ئ',
            [',']='و',['.']='.',['/']='/'
        }
    };
    public static readonly KeyboardLayoutDefinition EnglishToSorani = new()
    {
        Name = "English→Sorani",
        Map = new Dictionary<char, char>
        {
            ['q']='ق',['w']='و',['e']='ە',['r']='ر',['t']='ت',['y']='ی',['u']='و',['i']='ی',['o']='ۆ',['p']='پ',
            ['a']='ا',['s']='س',['d']='د',['f']='ف',['g']='گ',['h']='ه',['j']='ژ',['k']='ک',['l']='ل',
            ['z']='ز',['x']='خ',['c']='ج',['v']='ڤ',['b']='ب',['n']='ن',['m']='م',[',']='،',
            ['E']='ێ',['R']='ڕ',['S']='ش',['L']='ڵ',['C']='چ',
            ['1']='۱',['2']='۲',['3']='۳',['4']='۴',['5']='۵',['6']='۶',['7']='۷',['8']='۸',['9']='۹',['0']='۰'
        }
    };

    /// <summary>Remaps characters between keyboard layouts (e.g. English physical keys → Persian/Sorani letters).</summary>
    public static string Convert(string? input, KeyboardLayout from, KeyboardLayout to, KeyboardLayoutDefinition? custom = null)
    {
        if (from == to) return input ?? "";
        if (from == KeyboardLayout.English && to == KeyboardLayout.Persian) return Map(input, EnglishToPersian.Map);
        if (from == KeyboardLayout.Persian && to == KeyboardLayout.English)
            return Map(input, EnglishToPersian.Map.ToDictionary(x => x.Value, x => x.Key));
        if (from == KeyboardLayout.English && to == KeyboardLayout.Sorani) return Map(input, EnglishToSorani.Map);
        if (from == KeyboardLayout.Sorani && to == KeyboardLayout.English)
            return Map(input, EnglishToSorani.Map.GroupBy(x => x.Value).ToDictionary(g => g.Key, g => g.First().Key));
        if (custom is not null) return Map(input, custom.Map);
        return input ?? "";
    }
    private static string Map(string? s,IReadOnlyDictionary<char,char> map) => new((s??"").Select(c=>map.TryGetValue(c,out var x)?x:c).ToArray());
}

public static class Transliterator
{
    /// <summary>Persian/Arabic/Sorani letter → Latin phonetic segment.</summary>
    private static readonly Dictionary<char, string> Map = new()
    {
        ['ا']="a",['آ']="a",['أ']="a",['إ']="e",['ب']="b",['پ']="p",['ت']="t",['ث']="s",
        ['ج']="j",['چ']="ch",['ح']="h",['خ']="kh",['د']="d",['ذ']="z",['ر']="r",['ز']="z",
        ['ژ']="zh",['س']="s",['ش']="sh",['ص']="s",['ض']="z",['ط']="t",['ظ']="z",
        ['ع']="a",['غ']="gh",['ف']="f",['ق']="gh",['ک']="k",['ك']="k",['گ']="g",
        ['ل']="l",['م']="m",['ن']="n",['و']="u",['ه']="h",['ة']="h",['ۀ']="h",
        ['ی']="i",['ي']="i",['ى']="i",['ء']="",
        ['ێ']="ê",['ە']="e",['ڵ']="ll",['ڕ']="rr",['ڤ']="v",['ۆ']="o",['ۊ']="ü"
    };

    /// <summary>Transliterates Persian, Arabic or Sorani script to a Latin phonetic form.</summary>
    public static string ToLatin(string? input)
    {
        if (string.IsNullOrEmpty(input)) return input ?? "";
        var sb = new StringBuilder();
        foreach (var c in input)
            sb.Append(Map.TryGetValue(c, out var x) ? x : c.ToString());
        return sb.ToString();
    }

    /// <summary>Applies a custom multi-character replacement map (longest keys first).</summary>
    public static string Custom(string? input, IReadOnlyDictionary<string, string> map)
    {
        var s = input ?? "";
        foreach (var kv in map.OrderByDescending(x => x.Key.Length))
            s = s.Replace(kv.Key, kv.Value, StringComparison.Ordinal);
        return s;
    }
}

public static class SymbolConverter
{
    public static string Normalize(string? value) { var s=value??""; return s.Replace('٫','.').Replace('٬',',').Replace('،',',').Replace('؛',';').Replace('؟','?').Replace('٪','%').Replace('×','x'); }
}
