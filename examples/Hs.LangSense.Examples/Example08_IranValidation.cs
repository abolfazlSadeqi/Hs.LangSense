using Hs.LangSense;

namespace Hs.LangSense.Examples;

/// <summary>Iranian mobile, national code, bank card, IBAN validation.</summary>
public static class Example08_IranValidation
{
    public static void Run()
    {
        Console.WriteLine("【 08 — Iranian validation 】");
        Console.WriteLine($"  mobile     : {"09121111111".IsMobile()}  format={IranianMobile.Format("09121111111")}");
        Console.WriteLine($"  national   : {IranianNationalCode.IsValid("0011111111")}");
        var card = "6037991111111111";
        Console.WriteLine($"  bank card  : {BankCard.IsValid(card)}  bank={BankCard.DetectBank(card)}");
        Console.WriteLine($"  is card ext: {card.IsBankCard()}");
        Console.WriteLine();
    }
}
