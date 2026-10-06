using System.Net;
using System.Net.Mail;
namespace Hs.LangSense;
public sealed class DetectionResult { public ValueType Type{get;init;} public double Confidence{get;init;} public bool IsValid{get;init;} public string Normalized{get;init;}=""; public List<DetectionCandidate> Alternatives{get;init;}=[]; }
/// <summary>Prefer <see cref="SmartValue.Analyze"/> or <see cref="Text.Analyze"/>.</summary>
[System.Obsolete("Use SmartValue.Analyze or Text.Analyze.")]
public static class Detector
{
 public static DetectionResult Detect(string? input, object? context=null) {
  var s=Normalizer.Normalize(input); var c=new List<DetectionCandidate>();
  void Add(ValueType t,double conf,bool valid,string reason)=>c.Add(new(t,conf,valid,reason));
  if(string.IsNullOrWhiteSpace(s)){Add(ValueType.Text,.1,true,"Empty/text");}
  if(IranianMobile.TryParse(s,out _)) Add(ValueType.IranianMobile,.99,true,"Iranian mobile");
  else if(PhoneParser.TryParse(s,out _)) Add(ValueType.Phone,.9,true,"Phone parser");
  if(IranianNationalCode.IsValid(s)) Add(ValueType.NationalCode,.99,true,"Checksum");
  if(LegalNationalCodeValue.IsValid(s)) Add(ValueType.LegalNationalCode,.98,true,"Legal identifier checksum");
  if(IranianPostalCode.IsValid(s)) Add(ValueType.PostalCode,.95,true,"Postal code");
  if(BankCard.IsValid(s)) Add(ValueType.BankCard,.99,true,"Luhn");
  if(Iban.IsValid(s)) Add(ValueType.IBAN,.99,true,"IBAN checksum");
  if(Sayad.IsValid(s)) Add(ValueType.Sayad,.97,true,"Sayad length");
  if(EmailValue.IsValid(s)) Add(ValueType.Email,.98,true,"Email syntax");
  if(Uri.TryCreate(s,UriKind.Absolute,out var uri) && !string.IsNullOrEmpty(uri.Host)) Add(ValueType.Url,.96,true,"Absolute URI");
  if(IPAddress.TryParse(s,out var ip)) Add(ip.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork?ValueType.IPv4:ValueType.IPv6,.99,true,"IP parser");
  if(NumberParser.TryParse(s,out _)) Add(ValueType.Number,.8,true,"Number parser");
  if(DateParser.TryParse(s,out var parsedDate)) Add(parsedDate.Calendar==CalendarKind.Persian?ValueType.PersianDate:ValueType.GregorianDate,.9,true,"Date parser");
  if(TimeParser.TryParse(s,out _)) Add(ValueType.Time,.95,true,"Time parser");
  if(DateTimeParser.TryParse(s,out _)) Add(ValueType.DateTime,.9,true,"Date/time parser");
  if(DomainValue.IsValid(s)) Add(ValueType.Domain,.9,true,"Domain syntax");
  if(PercentageParser.TryParse(s,out _)) Add(ValueType.Percentage,.9,true,"Percentage");
  if(RatioParser.TryParse(s,out _)) Add(ValueType.Ratio,.92,true,"Ratio");
  if(YearParser.IsPersianYear(s) || YearParser.IsGregorianYear(s)) Add(ValueType.Year,.75,true,"Year");
  if(c.Count==0) Add(ValueType.Text,.5,true,"Fallback text");
  var best=c.OrderByDescending(x=>x.Confidence).First(); return new(){Type=best.Type,Confidence=best.Confidence,IsValid=best.IsValid,Normalized=s,Alternatives=c.OrderByDescending(x=>x.Confidence).ToList()};
 }
}
