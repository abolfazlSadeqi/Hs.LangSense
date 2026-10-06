namespace Hs.LangSense;

public sealed record SanitizationPolicy(bool Normalize=true,bool MaskSensitive=true,bool RedactScripts=false,bool RemoveControls=true);

public static class Sanitizer
{
    public static string Sanitize(string? input, SanitizationPolicy? policy=null)
    {
        policy ??= new(); var s=input??string.Empty;
        if(policy.Normalize) s=Normalizer.Normalize(s,new LangSenseOptions{RemoveControlCharacters=policy.RemoveControls});
        else if(policy.RemoveControls) s=new string(s.Where(c=>!char.IsControl(c)||c=='\n'||c=='\r'||c=='\t').ToArray());
        if(policy.RedactScripts) s=s.Replace("<script","[script",StringComparison.OrdinalIgnoreCase).Replace("</script>","[/script]",StringComparison.OrdinalIgnoreCase);
        return policy.MaskSensitive?SensitiveData.Mask(s):s;
    }
    public static string SanitizeForLog(string? input)=>LogScrubber.Scrub(input??string.Empty);
}
