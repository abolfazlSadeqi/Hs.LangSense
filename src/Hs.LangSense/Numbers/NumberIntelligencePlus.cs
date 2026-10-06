using System.Globalization;

namespace Hs.LangSense;

public sealed record RatioValue(decimal Numerator, decimal Denominator)
{
    public decimal Value => Denominator==0 ? 0 : Numerator/Denominator;
    public decimal Percentage => Value*100;
}

public static class RatioParser
{
    public static bool TryParse(string? input,out RatioValue value)
    {
        value=new(0,0); var s=Normalizer.Normalize(input).Replace(" ","");
        var parts=s.Split(':','/'); if(parts.Length!=2) return false;
        if(!NumberParser.TryParse(parts[0],out var a)||!NumberParser.TryParse(parts[1],out var b)||b==0) return false;
        value=new(a,b); return true;
    }
}

public static class NumberScale
{
    private static readonly Dictionary<string,decimal> Scales=new(StringComparer.OrdinalIgnoreCase)
    { ["k"]=1_000m,["هزار"]=1_000m,["m"]=1_000_000m,["میلیون"]=1_000_000m,["b"]=1_000_000_000m,["میلیارد"]=1_000_000_000m };
    public static bool TryParse(string? input,out decimal value)
    {
        value=0; var s=Normalizer.Normalize(input).Trim();
        var m=System.Text.RegularExpressions.Regex.Match(s,@"^(-?[0-9]+(?:\.[0-9]+)?)\s*([A-Za-z]+|هزار|میلیون|میلیارد)$");
        if(!m.Success||!decimal.TryParse(m.Groups[1].Value,NumberStyles.Number,CultureInfo.InvariantCulture,out var n)||!Scales.TryGetValue(m.Groups[2].Value,out var scale)) return false;
        value=n*scale; return true;
    }
}
