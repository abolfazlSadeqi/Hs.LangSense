namespace Hs.LangSense;

/// <summary>
/// Kurdish Sorani and Kurmanji number-word conversion.
/// </summary>
public static class KurdishWords
{
    private static readonly string[] SoraniOnes =
        ["", "یەک", "دوو", "سێ", "چوار", "پێنج", "شەش", "حەوت", "هەشت", "نۆ"];
    private static readonly string[] SoraniTeens =
        ["دە", "یازدە", "دوازدە", "سێزدە", "چواردە", "پازدە", "شازدە", "حەڤدە", "هەژدە", "نۆزدە"];
    private static readonly string[] SoraniTens =
        ["", "", "بیست", "سی", "چل", "پەنجا", "شەست", "حەفتا", "هەشتا", "نەوەت"];

    private static readonly string[] KurmanjiOnes =
        ["", "yek", "du", "sê", "çar", "pênc", "şeş", "heft", "heşt", "neh"];
    private static readonly string[] KurmanjiTeens =
        ["deh", "yazdeh", "dwazdeh", "sêzdeh", "çardeh", "pazdeh", "şazdeh", "hevdeh", "hejdeh", "nozdeh"];
    private static readonly string[] KurmanjiTens =
        ["", "", "bîst", "sî", "çil", "pêncî", "şêst", "heftê", "heştê", "nod"];

    private static readonly Dictionary<string, long> SoraniSmall = new(StringComparer.OrdinalIgnoreCase)
    {
        ["سفر"] = 0, ["یەک"] = 1, ["دوو"] = 2, ["سێ"] = 3, ["چوار"] = 4, ["پێنج"] = 5,
        ["شەش"] = 6, ["حەوت"] = 7, ["هەشت"] = 8, ["نۆ"] = 9,
        ["دە"] = 10, ["یازدە"] = 11, ["دوازدە"] = 12, ["سێزدە"] = 13, ["چواردە"] = 14,
        ["پازدە"] = 15, ["شازدە"] = 16, ["حەڤدە"] = 17, ["هەژدە"] = 18, ["نۆزدە"] = 19,
        ["بیست"] = 20, ["سی"] = 30, ["چل"] = 40, ["پەنجا"] = 50, ["شەست"] = 60,
        ["حەفتا"] = 70, ["هەشتا"] = 80, ["نەوەت"] = 90, ["سەد"] = 100
    };

    private static readonly Dictionary<string, long> SoraniScale = new(StringComparer.OrdinalIgnoreCase)
    {
        ["هەزار"] = 1000, ["هزار"] = 1000, ["ملیۆن"] = 1_000_000, ["میلیون"] = 1_000_000,
        ["ملیار"] = 1_000_000_000, ["میلیارد"] = 1_000_000_000
    };

    private static readonly Dictionary<string, long> KurmanjiSmall = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sifir"] = 0, ["yek"] = 1, ["du"] = 2, ["sê"] = 3, ["çar"] = 4, ["pênc"] = 5,
        ["şeş"] = 6, ["heft"] = 7, ["heşt"] = 8, ["neh"] = 9,
        ["deh"] = 10, ["yazdeh"] = 11, ["dwazdeh"] = 12, ["sêzdeh"] = 13, ["çardeh"] = 14,
        ["pazdeh"] = 15, ["şazdeh"] = 16, ["hevdeh"] = 17, ["hejdeh"] = 18, ["nozdeh"] = 19,
        ["bîst"] = 20, ["sî"] = 30, ["çil"] = 40, ["pêncî"] = 50, ["şêst"] = 60,
        ["heftê"] = 70, ["heştê"] = 80, ["nod"] = 90, ["sed"] = 100
    };

    private static readonly Dictionary<string, long> KurmanjiScale = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hezar"] = 1000, ["milyon"] = 1_000_000, ["milyar"] = 1_000_000_000
    };

    /// <summary>Converts an integer to Kurdish Sorani words.</summary>
    public static string ToSorani(long value)
    {
        if (value == 0) return "سفر";
        if (value < 0) return "نێرەتی " + ToSorani(-value);
        return string.Join(" و ", Build(value, SoraniOnes, SoraniTeens, SoraniTens, "سەد", "هەزار", "ملیۆن", "ملیار", " و "));
    }

    /// <summary>Converts an integer to Kurdish Kurmanji words.</summary>
    public static string ToKurmanji(long value)
    {
        if (value == 0) return "sifir";
        if (value < 0) return "negatîf " + ToKurmanji(-value);
        return string.Join(" û ", Build(value, KurmanjiOnes, KurmanjiTeens, KurmanjiTens, "sed", "hezar", "milyon", "milyar", " û "));
    }

    /// <summary>Parses Sorani number-words into a decimal.</summary>
    public static bool TryParseSorani(string? input, out decimal value)
        => TryParseWords(input, SoraniSmall, SoraniScale, out value);

    /// <summary>Parses Kurmanji number-words into a decimal.</summary>
    public static bool TryParseKurmanji(string? input, out decimal value)
        => TryParseWords(input, KurmanjiSmall, KurmanjiScale, out value);

    private static IEnumerable<string> Build(
        long value, string[] ones, string[] teens, string[] tens,
        string hundred, string thousand, string million, string billion, string joiner)
    {
        var parts = new List<string>();
        void AddScale(long scale, string name)
        {
            if (value < scale) return;
            var n = value / scale;
            parts.Add(BelowThousand(n, ones, teens, tens, hundred, joiner) + " " + name);
            value %= scale;
        }
        AddScale(1_000_000_000, billion);
        AddScale(1_000_000, million);
        AddScale(1000, thousand);
        if (value > 0) parts.Add(BelowThousand(value, ones, teens, tens, hundred, joiner));
        return parts;
    }

    private static string BelowThousand(long n, string[] ones, string[] teens, string[] tens, string hundred, string joiner)
    {
        var r = new List<string>();
        if (n >= 100)
        {
            var h = n / 100;
            r.Add(h == 1 ? hundred : ones[h] + " " + hundred);
            n %= 100;
        }
        if (n >= 20)
        {
            r.Add(tens[n / 10]);
            n %= 10;
            if (n > 0) r.Add(ones[n]);
        }
        else if (n >= 10) r.Add(teens[n - 10]);
        else if (n > 0) r.Add(ones[n]);
        return string.Join(joiner, r);
    }

    private static bool TryParseWords(
        string? input,
        Dictionary<string, long> small,
        Dictionary<string, long> scale,
        out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var s = input.Trim().Replace(" و ", " ").Replace(" û ", " ");
        long total = 0, current = 0;
        bool found = false;
        foreach (var token in s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (small.TryGetValue(token, out var n)) { current += n; found = true; }
            else if (scale.TryGetValue(token, out var sc))
            {
                if (current == 0) current = 1;
                total += current * sc; current = 0; found = true;
            }
            else return false;
        }
        value = total + current;
        return found;
    }
}
