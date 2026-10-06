namespace Hs.LangSense;

public sealed record ParseAttempt(ValueType Type, bool Success, double Confidence, string? Normalized, string? Error);

public static class UniversalParser
{
    public static ValueResult<T> Parse<T>(string? input, ValueType expected, Func<string,T?> parser, Func<T?,bool>? validator=null)
    {
        var normalized=Normalizer.Normalize(input); T? value;
        try { value=parser(normalized); }
        catch(Exception ex) { return new ValueResult<T>{Success=false,IsValid=false,RawValue=input,NormalizedValue=normalized,Type=expected,Errors=[new("PARSE_EXCEPTION",ex.Message)]}; }
        var valid=value is not null && (validator?.Invoke(value) ?? true);
        return new ValueResult<T>{Success=valid,Value=value,RawValue=input,NormalizedValue=normalized,CanonicalValue=Canonicalizer.Create(normalized),Type=expected,Confidence=valid?1:.4,IsValid=valid,Errors=valid?[]:[new("INVALID_VALUE",$"Could not parse as {expected}.")]};
    }

    public static IReadOnlyList<ParseAttempt> TryAll(string? input)
    {
        var s=Normalizer.Normalize(input); var attempts=new List<ParseAttempt>();
        attempts.Add(new(ValueType.IranianMobile,IranianMobile.IsValid(s),.99,IranianMobile.TryNormalize(s,out var mobile)?mobile:null,null));
        attempts.Add(new(ValueType.NationalCode,IranianNationalCode.IsValid(s),.99,IranianNationalCode.IsValid(s)?IranianNationalCode.Normalize(s):null,null));
        attempts.Add(new(ValueType.BankCard,BankCard.IsValid(s),.99,BankCard.IsValid(s)?BankCard.Normalize(s):null,null));
        attempts.Add(new(ValueType.IBAN,Iban.IsValid(s),.99,Iban.IsValid(s)?Iban.Normalize(s):null,null));
        attempts.Add(new(ValueType.Email,EmailValue.IsValid(s),.98,s,null));
        attempts.Add(new(ValueType.Url,UrlValue.IsValid(s),.97,s,null));
        attempts.Add(new(ValueType.Number,NumberParser.TryParse(s,out var n),.85,NumberParser.TryParse(s,out _)?n.ToString(System.Globalization.CultureInfo.InvariantCulture):null,null));
        attempts.Add(new(ValueType.Date,DateParser.TryParse(s,out var d),.8,DateParser.TryParse(s,out _)?d.Date.ToString("O"):null,null));
        return attempts.OrderByDescending(x=>x.Success).ThenByDescending(x=>x.Confidence).ToList();
    }
}
