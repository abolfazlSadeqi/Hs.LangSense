namespace Hs.LangSense.FluentValidation;

public static class LangSenseValidatorExtensions
{
    public static Func<string?, bool> PersianPhrase() =>
        s => s is not null && PersianTextUtilities.ContainsOnlyPersianLetters(s);
    public static Func<string?, bool> PersianOrEnglishPhrase()
        => s => s is not null && PersianTextUtilities.ContainsOnlyPersianOrEnglishLetters(s);
    public static Func<string?, bool> PersianLetters()
        => PersianPhrase();
    public static Func<string?, bool> PersianDateTime()
        => s => s is not null && Hs.LangSense.DateParser.TryParse(s, out _);
    public static Func<string?, bool> IranianNationalCode()
        => s => s is not null && Hs.LangSense.IranianNationalCode.IsValid(s);
    public static Func<string?, bool> IranianNationalLegalCode()
        => s => s is not null && LegalNationalCodeValue.IsValid(s);
    public static Func<string?, bool> IranianPostalCode()
        => s => s is not null && Hs.LangSense.IranianPostalCode.IsValid(s);
    public static Func<string?, bool> IranianMobileNumber() => s => s is not null && IranianMobile.IsValid(s);
    public static Func<string?, bool> IranianIbanNumber(params string[] bankCodes) => s => s is not null && Iban.IsValid(s) && (bankCodes.Length == 0 || bankCodes.Contains(Iban.BankCode(s), StringComparer.OrdinalIgnoreCase));
    public static Func<string?, bool> IranianShetabCardNumber(params string[] bins) => s => s is not null && BankCard.IsValid(s) && (bins.Length == 0 || bins.Any(b => BankCard.Normalize(s).StartsWith(b, StringComparison.Ordinal)));
}
