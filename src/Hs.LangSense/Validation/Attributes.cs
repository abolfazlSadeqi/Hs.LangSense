using System.ComponentModel.DataAnnotations;
namespace Hs.LangSense;
public sealed class MobileAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || IranianMobile.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid Iranian mobile.");
}
public sealed class NationalCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || IranianNationalCode.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid national code.");
}
public sealed class LegalNationalCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || LegalNationalCodeValue.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid legal national identifier.");
}
public sealed class PostalCodeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || IranianPostalCode.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid postal code.");
}
public sealed class IBANAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || Iban.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid IBAN.");
}
public sealed class BankCardAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || BankCard.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid bank card.");
}
public sealed class SayadAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || Sayad.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid Sayad identifier.");
}
public sealed class PersianTextAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || value.ToString()!.Any(PersianText.IsPersianLetter) ? ValidationResult.Success : new(ErrorMessage ?? "Persian text expected.");
}
public sealed class StudentScoreAttribute : RangeAttribute
{
    public StudentScoreAttribute() : base(0, 20) { ErrorMessage = "Student score must be between 0 and 20."; }
}
public sealed class PercentageAttribute : RangeAttribute
{
    public PercentageAttribute() : base(0, 100) { ErrorMessage = "Percentage must be between 0 and 100."; }
}
public sealed class MoneyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || Money.TryParse(value.ToString()!, out _) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid money value.");
}
public sealed class SmartValueAttribute : ValidationAttribute
{
    public ValueType Type { get; set; } = ValueType.Unknown;
    protected override ValidationResult? IsValid(object? value, ValidationContext c)
    {
        if (value is null)
            return ValidationResult.Success; var r = SmartValue.Analyze(value);
        return r.Type == Type && r.IsValid ? ValidationResult.Success : new(ErrorMessage ?? $"Value must be a valid {Type}.");
    }
}

public sealed class EmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || EmailValue.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid email.");
}
public sealed class UrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || UrlValue.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid URL.");
}
public sealed class AccountNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || AccountNumber.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid account number.");
}
public sealed class ChequeNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext c) => value is null || ChequeNumber.IsValid(value.ToString()) ? ValidationResult.Success : new(ErrorMessage ?? "Invalid cheque number.");
}
