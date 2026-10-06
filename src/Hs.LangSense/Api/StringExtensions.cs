namespace Hs.LangSense;

/// <summary>
/// Fluent extension methods for multilingual text conversion, normalization,
/// validation, extraction, search and masking.
/// </summary>
public static class StringExtensions
{
    // ── Language conversion (deterministic, not machine translation) ──

    /// <summary>Converts script/digits toward the target language.</summary>
    public static string ToLanguage(this string? value, Language language)
        => Text.Convert(value, language);

    /// <summary>Full deterministic conversion pipeline to the target language.</summary>
    public static string ConvertTo(this string? value, Language language)
        => Text.ConvertTo(value, language);

    /// <summary>Full deterministic conversion pipeline using a language code (fa, ar, en, sorani, kurmanji).</summary>
    public static string ConvertTo(this string? value, string language)
        => Text.ConvertTo(value, language);

    /// <summary>Prepare text for a target language (normalize + convert).</summary>
    public static string Prepare(this string? value, Language language = Language.Persian)
        => Text.Prepare(value, language);

    /// <summary>Convert toward Persian letters and digits.</summary>
    public static string ToPersian(this string? value) => Text.ToPersian(value);

    /// <summary>Convert toward Arabic letters and digits.</summary>
    public static string ToArabic(this string? value) => Text.ToArabic(value);

    /// <summary>Convert toward Latin/English representation (transliteration + Latin digits).</summary>
    public static string ToEnglish(this string? value) => Text.ToEnglish(value);

    /// <summary>Convert toward Kurdish Sorani script/digits.</summary>
    public static string ToSorani(this string? value) => Text.Convert(value, Language.Sorani);

    /// <summary>Convert toward Kurdish Kurmanji (Latin-oriented) representation.</summary>
    public static string ToKurmanji(this string? value) => Text.Convert(value, Language.Kurmanji);

    // ── Digits ──────────────────────────────────────────────

    /// <summary>Convert all digits to the requested number system.</summary>
    public static string ToDigits(this string? value, NumberSystem system) => Text.Digits(value, system);

    /// <summary>Convert all digits to Latin 0-9.</summary>
    public static string ToLatinDigits(this string? value) => Text.Digits(value, NumberSystem.Latin);

    /// <summary>Convert all digits to Persian ۰-۹.</summary>
    public static string ToPersianDigits(this string? value) => Text.Digits(value, NumberSystem.Persian);

    /// <summary>Convert all digits to Arabic-Indic ٠-٩.</summary>
    public static string ToArabicDigits(this string? value) => Text.Digits(value, NumberSystem.ArabicIndic);

    // ── Keyboard / transliteration ───────────────────────────

    /// <summary>Remap text typed with the wrong keyboard layout.</summary>
    public static string FixKeyboard(this string? value, KeyboardLayout from, KeyboardLayout to)
        => Text.Keyboard(value, from, to);

    /// <summary>Fix English-keyboard typing intended as Persian (e.g. jsj → تست).</summary>
    public static string FixPersianKeyboard(this string? value)
        => Text.Keyboard(value, KeyboardLayout.English, KeyboardLayout.Persian);

    /// <summary>Fix English-keyboard typing intended as Kurdish Sorani.</summary>
    public static string FixSoraniKeyboard(this string? value)
        => Text.Keyboard(value, KeyboardLayout.English, KeyboardLayout.Sorani);

    /// <summary>Deterministic transliteration to Latin characters.</summary>
    public static string Transliterate(this string? value) => Text.Transliterate(value);

    // ── Normalize / analyze / extract ───────────────────────

    /// <summary>
    /// LangSense normalize: Arabic→Persian letters (ي→ی، ك→ک), digits→Latin, remove ZWNJ, collapse whitespace.
    /// IMPORTANT: Do not use <c>string.Normalize()</c> — that is the BCL Unicode form method and will NOT apply Persian rules.
    /// </summary>
    public static string NormalizeText(this string? value) => Text.Normalize(value);

    /// <summary>Alias of <see cref="NormalizeText"/>. Prefer this over BCL <c>string.Normalize()</c>.</summary>
    public static string LangNormalize(this string? value) => Text.Normalize(value);

    /// <summary>Analyze the value: detect type, normalize, validate, explain.</summary>
    public static ValueResult<string> Analyze(this string? value, ValueContext? context = null)
        => Text.Analyze(value, context);

    /// <summary>Extract structured values (mobile, email, card, money, …) from free text.</summary>
    public static IReadOnlyList<TextOccurrence> ExtractValues(this string? value)
        => Text.Extract(value);

    // ── Search / compare ────────────────────────────────────

    /// <summary>Semantic equality (digits and look-alike letters unified).</summary>
    public static bool SameAs(this string? value, string? other) => Text.Same(value, other);

    /// <summary>Stable semantic fingerprint for search and deduplication.</summary>
    public static string Fingerprint(this string? value) => Text.Fingerprint(value);

    /// <summary>Canonical search key (normalized + folded).</summary>
    public static string ToSearchKey(this string? value) => Text.Fingerprint(value);

    // ── Security ────────────────────────────────────────────

    /// <summary>Mask sensitive data (mobile, card, IBAN, national code) inside text.</summary>
    public static string MaskSensitive(this string? value) => Text.Mask(value);

    /// <summary>Redact sensitive data (replace with a redaction token).</summary>
    public static string RedactSensitive(this string? value) => Text.Redact(value);

    /// <summary>Scrub sensitive data before writing logs.</summary>
    public static string ScrubLog(this string? value) => Text.ScrubLog(value);

    // ── Numbers / dates / money ─────────────────────────────

    /// <summary>Parse a natural-language or numeric date (including Persian expressions).</summary>
    public static DateTime? ToDate(this string? value, DateTime? now = null) => Text.Date(value, now);

    /// <summary>Try parse a number from multilingual digits or number-words.</summary>
    public static bool TryNumber(this string? value, out decimal result) => Text.Number(value, out result);

    /// <summary>Try parse a money amount (with optional currency unit words).</summary>
    public static bool TryMoney(this string? value, out MoneyValue result) => Text.Money(value, out result);

    /// <summary>Convert an integer to Persian number-words when available via Numbers engine.</summary>
    public static string ToPersianWords(this long value)
        => PersianWords.ToWords(value);

    /// <summary>Convert an integer to English number-words when available via Numbers engine.</summary>
    public static string ToEnglishWords(this long value)
        => NumberText.ToWords(value, Language.English);

    /// <summary>Convert an integer to Kurdish Sorani number-words.</summary>
    public static string ToSoraniWords(this long value) => KurdishWords.ToSorani(value);

    /// <summary>Convert an integer to Kurdish Kurmanji number-words.</summary>
    public static string ToKurmanjiWords(this long value) => KurdishWords.ToKurmanji(value);

    /// <summary>Convert an integer to number-words in the given language.</summary>
    public static string ToWords(this long value, Language language)
        => NumberText.ToWords(value, language);

    // ── Iranian validation shortcuts ────────────────────────

    /// <summary>True if the value is a valid Iranian mobile number.</summary>
    public static bool IsMobile(this string? value) => IranianMobile.IsValid(value);

    /// <summary>True if the value is a valid Iranian national code.</summary>
    public static bool IsNationalCode(this string? value) => IranianNationalCode.IsValid(value);

    /// <summary>True if the value is a valid Iranian postal code.</summary>
    public static bool IsPostalCode(this string? value) => IranianPostalCode.IsValid(value);

    /// <summary>True if the value is a valid Shetab bank card (Luhn).</summary>
    public static bool IsBankCard(this string? value) => BankCard.IsValid(value);

    /// <summary>True if the value is a valid Iranian IBAN / Sheba.</summary>
    public static bool IsIban(this string? value) => Iban.IsValid(value);

    /// <summary>True if the value is a valid email address.</summary>
    public static bool IsEmail(this string? value) => EmailValue.IsValid(value);

    /// <summary>True if the value is a valid absolute URL.</summary>
    public static bool IsUrl(this string? value) => UrlValue.IsValid(value);
}
