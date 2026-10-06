namespace Hs.LangSense;

public sealed record LanguageConversionResult(string Input, string Output, Language From, Language To, double Confidence, IReadOnlyList<Transformation> Transformations);

/// <summary>Prefer <see cref="Text.Convert"/> / <see cref="Text.ConvertTo"/>. This type remains for compatibility.</summary>
[System.Obsolete("Use Text.Convert or Text.ConvertTo instead.")]
public static class UniversalLanguageConverter
{
    public static LanguageConversionResult Convert(string? input, Language from, Language to, IReadOnlyDictionary<string,string>? custom = null)
    {
        var source = input ?? string.Empty;
        if (from == to) return new(source, source, from, to, 1, Array.Empty<Transformation>());
        string output = source;
        if (custom is not null) output = Transliterator.Custom(output, custom);
        else if (to == Language.English && from is Language.Persian or Language.Arabic) output = Transliterator.ToLatin(output);
        else if (to == Language.Persian && from == Language.Arabic) output = Normalizer.NormalizePersian(output);
        else if (to == Language.Persian && from == Language.English) output = KeyboardLayouts.Convert(output, KeyboardLayout.English, KeyboardLayout.Persian);
        var transformation = new Transformation(TransformationKind.Custom, $"{from}→{to}", source, output, true);
        return new(source, output, from, to, output == source ? .5 : .9, new[] { transformation });
    }
}

/// <summary>Prefer <see cref="KeyboardLayouts"/> and <see cref="Text.Keyboard"/>.</summary>
[System.Obsolete("Use KeyboardLayouts / Text.Keyboard instead.")]
public static class KeyboardLayoutCatalog
{
    public static KeyboardLayoutDefinition Persian { get; } = KeyboardLayouts.EnglishToPersian;
    public static KeyboardLayoutDefinition Arabic { get; } = new KeyboardLayoutDefinition
    {
        Name = "English→Arabic",
        Map = new Dictionary<char,char>
        {
            ['q']='ض',['w']='ص',['e']='ث',['r']='ق',['t']='ف',['y']='غ',['u']='ع',['i']='ه',['o']='خ',['p']='ح',
            ['a']='ش',['s']='س',['d']='ي',['f']='ب',['g']='ل',['h']='ا',['j']='ت',['k']='ن',['l']='م',
            ['z']='ئ',['x']='ء',['c']='ؤ',['v']='ر',['b']='ل',['n']='ى',['m']='ة'
        }
    };
}
