namespace Hs.LangSense.Formatting;

public sealed record FormatOptions(string? Culture = null, bool PersianDigits = false, bool GroupThousands = true, int? DecimalPlaces = null, bool MaskSensitive = false);

public static class ContextFormatter
{
    public static string Format(string? value, ValueType type, FormatOptions? options = null)
    {
        options ??= new FormatOptions();
        var text = value ?? string.Empty;
        var result = type switch
        {
            ValueType.IranianMobile => IranianMobile.Format(text),
            ValueType.BankCard => BankCard.Format(text),
            ValueType.IBAN => Iban.Format(text),
            ValueType.Money => Money.TryParse(text, out var m) ? m.Format(options.DecimalPlaces) : text,
            _ => text
        };
        if (options.MaskSensitive) result = SensitiveData.Mask(result);
        if (options.PersianDigits) result = DigitConverter.ToPersian(result);
        return result;
    }
}
