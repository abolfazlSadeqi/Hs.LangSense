using System.Text.RegularExpressions;
namespace Hs.LangSense;
public sealed class ValuePattern
{
 public string Name{get;} public Regex Regex{get;} public ValueType Type{get;} public IReadOnlyList<IRule> Rules{get;}
 public ValuePattern(string name,string regex,ValueType type=ValueType.Unknown,IEnumerable<IRule>? rules=null){Name=name;Regex=new(regex,RegexOptions.Compiled|RegexOptions.IgnoreCase);Type=type;Rules=rules?.ToList()??[];}
 public bool IsMatch(string value)=>Regex.IsMatch(value);
 public IEnumerable<PatternMatch> Extract(string text){foreach(Match m in Regex.Matches(text)) {var issues=Rules.Select(r=>r.Validate(m.Value)).Where(x=>x!=null).Cast<ValidationIssue>().ToList();yield return new(Name,m.Value,m.Index,m.Length,Type,issues.Count==0,issues);}}
}
public sealed record PatternMatch(string Pattern,string Value,int Start,int Length,ValueType Type,bool IsValid,IReadOnlyList<ValidationIssue> Issues);
public static class Patterns
{
 public static ValuePattern Mobile=>new("Mobile",@"(?<!\d)(?:\+98|0098|98|0)?9\d{9}(?!\d)",ValueType.IranianMobile,[Rules.Mobile]);
 public static ValuePattern Email=>new("Email",@"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}",ValueType.Email,[Rules.Email]);
 public static ValuePattern IBAN=>new("IBAN",@"\bIR\d{24}\b",ValueType.IBAN,[Rules.IBAN]);
 public static ValuePattern BankCard=>new("BankCard",@"(?<!\d)(?:\d[ -]?){16}(?!\d)",ValueType.BankCard,[Rules.BankCard]);
 public static ValuePattern NationalCode=>new("NationalCode",@"(?<!\d)\d{10}(?!\d)",ValueType.NationalCode,[Rules.NationalCode]);
 public static ValuePattern PostalCode=>new("PostalCode",@"(?<!\d)\d{10}(?!\d)",ValueType.PostalCode,[Rules.PostalCode]);
 public static ValuePattern Url=>new("Url",@"https?://[^\s]+",ValueType.Url,[Rules.Url]);
 public static ValuePattern IPv4=>new("IPv4",@"(?<![\d.])(?:\d{1,3}\.){3}\d{1,3}(?![\d.])",ValueType.IPv4);
 public static ValuePattern Money=>new("Money",@"(?:\d[\d,\.]*|[۰-۹][۰-۹٬٫]*)\s*(?:تومان|ریال|هزار|میلیون|میلیارد|billion|million|thousand)",ValueType.Money);
 public static ValuePattern LegalNationalCode=>new("LegalNationalCode",@"(?<!\d)\d{11}(?!\d)",ValueType.LegalNationalCode,[Rules.LegalNationalCode]);
 public static ValuePattern Sayad=>new("Sayad",@"(?<!\d)\d{16}(?!\d)",ValueType.Sayad,[Rules.Regex(@"^\d{16}$")]);
 public static ValuePattern ChequeNumber=>new("ChequeNumber",@"(?<!\d)\d{6,20}(?!\d)",ValueType.ChequeNumber,[Rules.Regex(@"^\d{6,20}$")]);
 public static ValuePattern AccountNumber=>new("AccountNumber",@"(?<!\d)\d{8,24}(?!\d)",ValueType.AccountNumber);
 public static ValuePattern IPv6=>new("IPv6",@"(?i)(?<![0-9a-f:])(?:[0-9a-f]{1,4}:){2,7}[0-9a-f]{1,4}(?![0-9a-f:])",ValueType.IPv6);
 public static ValuePattern Date=>new("Date",@"(?<!\d)(?:1[34]\d{2}|\d{4})[/-]\d{1,2}[/-]\d{1,2}(?!\d)",ValueType.Date);
 public static ValuePattern Percentage=>new("Percentage",@"(?:\d+(?:\.\d+)?|[۰-۹]+(?:٫[۰-۹]+)?)\s*(?:%|٪|درصد)",ValueType.Percentage);
}
public sealed class PatternBuilder
{
 private readonly List<string> _parts=[]; private readonly List<IRule> _rules=[]; private string _name="Custom"; private ValueType _type=ValueType.Unknown;
 public static PatternBuilder Create(string name)=>new(){_name=name}; public PatternBuilder Type(ValueType type){_type=type;return this;} public PatternBuilder StartsWith(string s){_parts.Add(Regex.Escape(s));return this;} public PatternBuilder EndsWith(string s){_parts.Add(Regex.Escape(s)+"$");return this;} public PatternBuilder Literal(string s){_parts.Add(Regex.Escape(s));return this;} public PatternBuilder Digits(int count){_parts.Add($@"\d{{{count}}}");return this;} public PatternBuilder Digits(int min,int max){_parts.Add($@"\d{{{min},{max}}}");return this;} public PatternBuilder Letters(int min,int max){_parts.Add($@"[A-Za-z]{{{min},{max}}}");return this;} public PatternBuilder Any(int min,int max){_parts.Add($@".{{{min},{max}}}");return this;} public PatternBuilder Optional(string s){_parts.Add($@"(?:{Regex.Escape(s)})?");return this;} public PatternBuilder OneOf(params string[] values){_parts.Add("(?:"+string.Join("|",values.Select(Regex.Escape))+")");return this;} public PatternBuilder Checksum(IRule rule){_rules.Add(rule);return this;} public PatternBuilder Rule(IRule rule){_rules.Add(rule);return this;} public ValuePattern Build(){var body=string.Concat(_parts); if(!body.StartsWith("^")) body="^"+body; if(!body.EndsWith("$")) body=body+"$"; return new(_name,body,_type,_rules);}
}
public static class PatternRegistry { private static readonly Dictionary<string,ValuePattern> Items=new(StringComparer.OrdinalIgnoreCase); public static void Register(ValuePattern p)=>Items[p.Name]=p; public static ValuePattern? Get(string name)=>Items.TryGetValue(name,out var p)?p:null; public static IReadOnlyCollection<ValuePattern> All=>Items.Values; public static void RegisterDefaults(){foreach(var p in new[]{Patterns.Mobile,Patterns.Email,Patterns.IBAN,Patterns.BankCard,Patterns.NationalCode,Patterns.PostalCode,Patterns.Url,Patterns.IPv4,Patterns.IPv6,Patterns.Date,Patterns.LegalNationalCode,Patterns.Sayad,Patterns.ChequeNumber,Patterns.AccountNumber,Patterns.Money,Patterns.Percentage}) Register(p);} }
