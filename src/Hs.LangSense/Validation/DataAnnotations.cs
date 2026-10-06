using System.ComponentModel.DataAnnotations;

namespace Hs.LangSense.DataAnnotations;

public abstract class LangSenseValidationAttribute : ValidationAttribute
{
    protected abstract bool IsValidValue(string value);
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        => value is null || IsValidValue(value.ToString() ?? string.Empty) ? ValidationResult.Success : new ValidationResult(ErrorMessage ?? $"Invalid {validationContext.MemberName}.");
}
public class PersianLettersAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => value.All(c => PersianText.IsPersianLetter(c) || char.IsWhiteSpace(c));
}
public sealed class PersianPhraseAttribute : PersianLettersAttribute { }
public sealed class PersianOrEnglishPhraseAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => PersianTextUtilities.ContainsOnlyPersianOrEnglishLetters(value);
}
public sealed class IranianMobileNumberAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => IranianMobile.IsValid(value);
}
public sealed class IranianNationalCodeAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => IranianNationalCode.IsValid(value);
}
public sealed class IranianNationalLegalCodeAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => LegalNationalCodeValue.IsValid(value);
}
public sealed class IranianPostalCodeAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => IranianPostalCode.IsValid(value);
}
public sealed class IranianIbanAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => Iban.IsValid(value);
}
public sealed class IranianShetabCardAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => BankCard.IsValid(value);
}
public sealed class PercentageAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) =>
        PercentageParser.TryParse(value, out var n) && n is >= 0 and <= 100;
}
public sealed class StudentScoreAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) =>
        ScoreParser.TryParse(value, out var score, out var max) && score is >= 0 && score <= max && max <= 100;
}
public sealed class MoneyAttribute : LangSenseValidationAttribute
{
    protected override bool IsValidValue(string value) => Money.TryParse(value, out _);
}
