namespace Hs.LangSense.Extensions;

/// <summary>
/// Convenience coercion helpers for Iranian identifiers (normalize + validate).
/// </summary>
public static class IranianCoercionExtensions
{
    /// <summary>
    /// Normalizes an Iranian mobile number to national form (09xxxxxxxxx).
    /// Returns empty string when the value is not a valid mobile.
    /// Accepts +98, 0098, 9xxxxxxxxx and multi-script digits.
    /// </summary>
    public static string CoerceIranianMobileNumber(this string? value)
        => IranianMobile.TryNormalize(value, out var n) ? n : string.Empty;

    /// <summary>
    /// Normalizes an Iranian national code to 10 Latin digits, or empty if invalid.
    /// </summary>
    public static string CoerceIranianNationalCode(this string? value)
        => IranianNationalCode.IsValid(value) ? IranianNationalCode.Normalize(value!) : string.Empty;

    /// <summary>
    /// Normalizes a Shetab card number (16 Latin digits) or empty if invalid.
    /// </summary>
    public static string CoerceBankCard(this string? value)
        => BankCard.IsValid(value) ? BankCard.Normalize(value!) : string.Empty;

    /// <summary>
    /// Normalizes an Iranian IBAN / Sheba or empty if invalid.
    /// </summary>
    public static string CoerceIban(this string? value)
        => Iban.IsValid(value) ? Iban.Normalize(value!) : string.Empty;
}
