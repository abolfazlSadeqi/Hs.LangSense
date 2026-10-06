namespace Hs.LangSense;

/// <summary>
/// Iranian bank identifiers: IBAN bank codes and card BINs (IIN).
/// </summary>
public static class IranBankConstants
{
    /// <summary>Iranian bank identifiers used in IBAN (Sheba).</summary>
    public static class BankCodes
    {
        public const string CentralBank = "010";
        public const string SanatOMadan = "011";
        public const string Mellat = "012";
        public const string Refah = "013";
        public const string Maskan = "014";
        public const string Sepah = "015";
        public const string Keshavarzi = "016";
        public const string Melli = "017";
        public const string Tejarat = "018";
        public const string Saderat = "019";
        public const string ToseeSaderat = "020";
        public const string Post = "021";
        public const string ToseeTaavon = "022";
        public const string Tosee = "051";
        public const string Karafarin = "053";
        public const string Parsian = "054";
        public const string EghtesadNovin = "055";
        public const string Saman = "056";
        public const string Pasargad = "057";
        public const string Sarmayeh = "058";
        public const string Sina = "059";
        public const string MehrIran = "060";
        public const string Shahr = "061";
        /// <summary>Historical. Ayandeh license revoked 2025; obligations transferred to Melli.</summary>
        public const string AyandehLegacy = "062";
        public const string Gardeshgari = "064";
        public const string Dey = "066";
        public const string IranZamin = "069";
        public const string Resalat = "070";
        public const string Melal = "075";
        public const string MiddleEast = "078";
        public const string NoorLegacy = "080";
        public const string IranVenezuela = "095";
    }

    /// <summary>First 6 digits of Iranian bank cards (BIN/IIN). Some banks have multiple BINs.</summary>
    public static class BankCardBins
    {
        public const string Melli = "603799";
        public const string Mellat = "610433";
        public const string MellatLegacy = "991975";
        public const string Saderat = "603769";
        public const string Tejarat = "627353";
        public const string TejaratLegacy = "585983";
        public const string Sepah = "589210";
        public const string Refah = "589463";
        public const string Keshavarzi = "603770";
        public const string KeshavarziLegacy = "639217";
        public const string Maskan = "628023";
        public const string SanatOMadan = "627961";
        public const string ToseeSaderat = "627648";
        public const string ToseeSaderatLegacy = "207177";
        public const string Post = "627760";
        public const string ToseeTaavon = "502908";
        public const string EghtesadNovin = "627412";
        public const string Parsian = "622106";
        public const string Parsian2 = "627884";
        public const string Parsian3 = "639194";
        public const string Pasargad = "502229";
        public const string Pasargad2 = "639347";
        public const string Karafarin = "627488";
        public const string Karafarin2 = "502910";
        public const string Saman = "621986";
        public const string Sarmayeh = "639607";
        public const string Sina = "639346";
        public const string Shahr = "502806";
        public const string Shahr2 = "504706";
        public const string Dey = "502938";
        public const string IranZamin = "505785";
        public const string Gardeshgari = "505416";
        public const string Gardeshgari2 = "505426";
        public const string MehrIran = "606373";
        public const string Resalat = "504172";
        public const string MiddleEast = "585947";
        public const string Melal = "606256";
        public const string Tosee = "628157";
        public const string NoorLegacy = "507677";
        public const string CentralBank = "636795";
        public const string AyandehLegacy = "636214";
        public const string AnsarLegacy = "627381";
        public const string HekmatIranianLegacy = "636949";
        public const string MehrEqtesadLegacy = "639370";
        public const string GhavaminLegacy = "639599";
        public const string KosarLegacy = "505801";
        /// <summary>Historical Melli BIN — not primary.</summary>
        public const string MelliLegacy = "170019";
    }
}
