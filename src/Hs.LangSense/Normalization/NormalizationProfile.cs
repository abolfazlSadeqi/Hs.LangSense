namespace Hs.LangSense;

public sealed record NormalizationProfile(string Name, LangSenseOptions Options)
{
    public static NormalizationProfile Strict => new("Strict", new LangSenseOptions
    {
        RemoveControlCharacters = true,
        RemoveDiacritics = false,
        PreserveZeroWidthNonJoiner = true,
        PreserveZeroWidthJoiner = false,
        PreserveLeftRightMarks = false
    });
    public static NormalizationProfile Search => new("Search", new LangSenseOptions
    {
        RemoveControlCharacters = true,
        RemoveDiacritics = true,
        PreserveZeroWidthNonJoiner = false,
        PreserveZeroWidthJoiner = false,
        PreserveLeftRightMarks = false
    });
    public static NormalizationProfile PersianText => new("PersianText", new LangSenseOptions
    {
        ConvertPersianDigitsToLatin = false,
        ConvertArabicDigitsToLatin = false,
        NormalizeArabicLetters = true,
        PreserveZeroWidthNonJoiner = true,
        RemoveDiacritics = false
    });
    public static NormalizationProfile Machine => new("Machine", new LangSenseOptions
    {
        RemoveControlCharacters = true,
        PreserveZeroWidthNonJoiner = false,
        PreserveZeroWidthJoiner = false,
        PreserveLeftRightMarks = false
    });
}

public static class NormalizationProfiles
{
    public static string Apply(string? value, NormalizationProfile profile) => Normalizer.Normalize(value, profile.Options);
}
