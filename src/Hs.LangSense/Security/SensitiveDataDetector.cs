namespace Hs.LangSense;

public sealed record SensitiveOccurrence(ValueType Type, string Value, int Start, int Length, double Confidence, string MaskedValue);

/// <summary>Detects sensitive values in free text and returns masked previews.</summary>
public static class SensitiveDataDetector
{
    public static IReadOnlyList<SensitiveOccurrence> Find(string? text)
    {
        var input = text ?? string.Empty;
        return TextExtractor.Extract(input)
            .Where(x => x.Type is ValueType.BankCard or ValueType.IBAN or ValueType.IranianMobile
                or ValueType.NationalCode or ValueType.Email)
            .Select(x => new SensitiveOccurrence(
                x.Type, x.Value, x.Start, x.Length, x.Confidence, MaskFor(x.Type, x.Value)))
            .ToList();
    }

    private static string MaskFor(ValueType type, string value) => type switch
    {
        ValueType.BankCard => BankCard.Mask(value),
        ValueType.IBAN => Iban.Mask(value),
        ValueType.IranianMobile => IranianMobile.Mask(value),
        ValueType.NationalCode => IranianNationalCode.Mask(value),
        ValueType.Email => value.Contains("@") ? value[0] + "***@" + value.Split("@")[^1] : "***",
        _ => value.Length > 4 ? new string('*', value.Length - 4) + value[^4..] : "****"
    };
}
