namespace Hs.LangSense;
public sealed record BankValue(ValueType Type, string Normalized, string Formatted, bool Valid, string? Bank, string? BankCode, string Masked);
public static class IranianBank
{
    public static BankValue Parse(string value)
    {
        var s = Normalizer.Normalize(value);
        if (BankCard.IsValid(s))
        {
            var n = BankCard.Normalize(s); var prefix = n.Length >= 6 ? n[..6] : "";
            return new(ValueType.BankCard, n, BankCard.Format(n), true, BankCatalog.TryGetName(prefix), prefix, BankCard.Mask(n));
        }
        if (Iban.IsValid(s))
        {
            var n = Iban.Normalize(s);
            var code = Iban.BankCode(n);
            return new(ValueType.IBAN, n, Iban.Format(n), true, code is null ? null : BankCatalog.TryGetName(code), code, Iban.Mask(n));
        }
        if (Sayad.IsValid(s))
            return new(ValueType.Sayad, Sayad.Normalize(s), Sayad.Normalize(s), true, null, null, new string('*', Math.Max(0, s.Length - 4)) + s[^4..]);

        if (ChequeNumber.IsValid(s))
            return new(ValueType.ChequeNumber, ChequeNumber.Normalize(s), ChequeNumber.Normalize(s), true, null, null, "***" + s[^4..]);
        return new(ValueType.Unknown, s, s, false, null, null, "***");
    }
}
