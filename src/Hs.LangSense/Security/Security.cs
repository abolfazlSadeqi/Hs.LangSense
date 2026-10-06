namespace Hs.LangSense;

public static class MaskEngine
{
    public static string Mask(string? value,string pattern,char placeholder='#')
    {
        var s=value??string.Empty; var i=0;
        return new string(pattern.Select(c=>c==placeholder&&i<s.Length?s[i++]:c).ToArray());
    }
}

public static class SensitiveData
{
    public static IReadOnlyList<TextOccurrence> Detect(string text)
    {
        var all=TextExtractor.Extract(text).Where(x=>x.Type is ValueType.BankCard or ValueType.IBAN or ValueType.NationalCode or ValueType.IranianMobile or ValueType.Email or ValueType.AccountNumber or ValueType.Sayad or ValueType.ChequeNumber).OrderByDescending(Priority).ThenByDescending(x=>x.Length).ThenBy(x=>x.Start).ToList();
        var selected=new List<TextOccurrence>();
        foreach(var item in all) if(!selected.Any(x=>Overlaps(x,item))) selected.Add(item);
        return selected.OrderBy(x=>x.Start).ToList();
    }
    public static string Mask(string text)
    {
        foreach (var x in Detect(text).OrderByDescending(x => x.Start))
        {
            // Use the exact span from the source text so surrounding spaces are preserved.
            var span = text.Substring(x.Start, Math.Min(x.Length, text.Length - x.Start));
            var core = span.Trim();
            var leading = span.Length - span.TrimStart().Length;
            var trailing = span.Length - span.TrimEnd().Length;
            var maskedCore = x.Type switch
            {
                ValueType.BankCard => BankCard.Mask(core),
                ValueType.IBAN => Iban.Mask(core),
                ValueType.NationalCode => IranianNationalCode.Mask(core),
                ValueType.IranianMobile => IranianMobile.Mask(core),
                ValueType.Email => EmailMask(core),
                ValueType.Sayad => GenericMask(core),
                ValueType.ChequeNumber => GenericMask(core),
                _ => GenericMask(core)
            };
            var replacement = new string(' ', leading) + maskedCore + new string(' ', trailing);
            text = text.Remove(x.Start, x.Length).Insert(x.Start, replacement);
        }
        return text;
    }
    public static string Redact(string text)
    {
        foreach (var x in Detect(text).OrderByDescending(x => x.Start))
        {
            var span = text.Substring(x.Start, Math.Min(x.Length, text.Length - x.Start));
            var leading = span.Length - span.TrimStart().Length;
            var trailing = span.Length - span.TrimEnd().Length;
            var replacement = new string(' ', leading) + "[REDACTED]" + new string(' ', trailing);
            text = text.Remove(x.Start, x.Length).Insert(x.Start, replacement);
        }
        return text;
    }
    private static int Priority(TextOccurrence x)=>x.Type switch {ValueType.BankCard=>100,ValueType.IBAN=>95,ValueType.IranianMobile=>90,ValueType.NationalCode=>85,ValueType.Email=>80,ValueType.Sayad=>75,ValueType.ChequeNumber=>70,_=>50};
    private static bool Overlaps(TextOccurrence a,TextOccurrence b)=>a.Start<b.Start+b.Length&&b.Start<a.Start+a.Length;
    private static string GenericMask(string s)=>s.Length>4?new string('*',s.Length-4)+s[^4..]:"****";
    private static string EmailMask(string s)=>EmailValue.TryParse(s,out var e)?(e.Username.Length>1?e.Username[0]+"***":"*")+"@"+e.Domain:"***";
}

public static class LogScrubber { public static string Scrub(string? text)=>SensitiveData.Mask(text??string.Empty); }
