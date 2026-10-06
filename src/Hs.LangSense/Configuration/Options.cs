namespace Hs.LangSense;
public sealed class DetectionPolicy { public double MinimumConfidence{get;set;}=.5;public bool IncludeAlternatives{get;set;}=true; }
public sealed class SecurityPolicy { public bool MaskCards{get;set;}=true;public bool MaskNationalCodes{get;set;}=true;public bool MaskMobiles{get;set;}=true;public bool MaskEmails{get;set;}=true; }
public sealed class PhonePolicy { public bool AcceptInternational{get;set;}=true;public bool AcceptSeparators{get;set;}=true; }
public sealed class PersianDigitPolicy { public NumberSystem Output{get;set;}=NumberSystem.Latin; }
public sealed class LangSenseConfiguration { public LangSenseOptions Values{get;set;}=new();public DetectionPolicy Detection{get;set;}=new();public SecurityPolicy Security{get;set;}=new();public PhonePolicy Phone{get;set;}=new();public PersianDigitPolicy Digits{get;set;}=new(); }
