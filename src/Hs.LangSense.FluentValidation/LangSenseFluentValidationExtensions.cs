using FluentValidation;

namespace Hs.LangSense.FluentValidation;

/// <summary>FluentValidation rules backed by LangSense domain validators.</summary>
public static class LangSenseFluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> PersianPhrase<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && PersianTextUtilities.ContainsOnlyPersianLetters(x));

    public static IRuleBuilderOptions<T, string?> PersianOrEnglishPhrase<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && PersianTextUtilities.ContainsOnlyPersianOrEnglishLetters(x));

    public static IRuleBuilderOptions<T, string?> PersianLetters<T>(this IRuleBuilder<T, string?> rule)
        => rule.PersianPhrase();

    public static IRuleBuilderOptions<T, string?> PersianDateTimeRule<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && DateParser.TryParse(x, out _));

    public static IRuleBuilderOptions<T, string?> IranianNationalCode<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && Hs.LangSense.IranianNationalCode.IsValid(x));

    public static IRuleBuilderOptions<T, string?> IranianNationalLegalCode<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && LegalNationalCodeValue.IsValid(x));

    public static IRuleBuilderOptions<T, string?> IranianPostalCode<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && Hs.LangSense.IranianPostalCode.IsValid(x));

    public static IRuleBuilderOptions<T, string?> IranianMobileNumber<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && IranianMobile.IsValid(x));

    public static IRuleBuilderOptions<T, string?> IranianIbanNumber<T>(this IRuleBuilder<T, string?> rule, params string[] bankCodes)
        => rule.Must(x => x is not null && Iban.IsValid(x)
            && (bankCodes.Length == 0 || bankCodes.Contains(Iban.BankCode(x), StringComparer.OrdinalIgnoreCase)));

    public static IRuleBuilderOptions<T, string?> IranianShetabCardNumber<T>(this IRuleBuilder<T, string?> rule, params string[] bins)
        => rule.Must(x => x is not null && Hs.LangSense.BankCard.IsValid(x)
            && (bins.Length == 0 || bins.Any(b => Hs.LangSense.BankCard.Normalize(x).StartsWith(b, StringComparison.Ordinal))));

    public static IRuleBuilderOptions<T, string?> PercentageValue<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && PercentageParser.TryParse(x, out var n) && n is >= 0 and <= 100);

    public static IRuleBuilderOptions<T, string?> MoneyValue<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && Hs.LangSense.Money.TryParse(x, out _));

    /// <summary>Validates Iranian Shetab bank card (Luhn). Named to avoid clash with type <see cref="Hs.LangSense.BankCard"/>.</summary>
    public static IRuleBuilderOptions<T, string?> IranianBankCard<T>(this IRuleBuilder<T, string?> rule)
        => rule.Must(x => x is not null && Hs.LangSense.BankCard.IsValid(x));
}
