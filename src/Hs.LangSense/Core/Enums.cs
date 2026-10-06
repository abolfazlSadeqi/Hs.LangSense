namespace Hs.LangSense;
public enum ValueType { Unknown, Text, Number, Integer, Decimal, Money, Cheque, Percentage, Ratio, Score, Age, Year, Month, Day, Date, PersianDate, GregorianDate, Time, DateTime, Email, Url, Domain, IPv4, IPv6, Phone, IranianMobile, NationalCode, LegalNationalCode, PostalCode, BankCard, IBAN, AccountNumber, Sayad, ChequeNumber, Identifier }
public enum ValidationMode { Strict, Relaxed, Normalized }
public enum ComparisonMode { Exact, Normalized, Canonical, Semantic, Search }
public enum Language { Unknown, Persian, Arabic, English, Latin, Sorani, Kurmanji, Numeric, Mixed }
public enum ConfidenceLevel { Unknown, Low, Medium, High, Certain }
public enum TransformationKind { Unicode, Digit, Letter, Whitespace, Punctuation, Keyboard, Transliteration, Normalization, Formatting, Masking, Redaction, Custom }
public enum MaskType { Generic, Mobile, BankCard, IBAN, NationalCode, Email, Account }
public enum NumberSystem { Latin, Persian, ArabicIndic, Custom }
public enum CalendarKind { Gregorian, Persian, Arabic }
public enum DetectorStage { Syntax, Normalization, Context, Validation, Heuristic }
