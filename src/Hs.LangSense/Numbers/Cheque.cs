namespace Hs.LangSense;

public sealed record ChequeValue(decimal Amount, Currency Currency, string Words, string Normalized, bool IsValid);

public static class Cheque
{
    public static bool TryParse(string? input, out ChequeValue value)
    {
        value = new(0, Currency.Unknown, string.Empty, string.Empty, false);
        if (!Money.TryParse(input ?? string.Empty, out var money)) return false;
        var normalized = Normalizer.Normalize(input); var words = PersianWords.ToWords((long)money.Amount) + " تمام";
        value = new(money.Amount, money.Currency, words,
            money.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture), true);
        return true;
    }
    public static string ToWords(decimal amount) => PersianWords.ToWords((long)amount) + " تمام";
    public static string Format(decimal amount, Currency currency = Currency.Toman) =>
        $"{Money.Format(amount, currency)} — {ToWords(amount)}";
}
