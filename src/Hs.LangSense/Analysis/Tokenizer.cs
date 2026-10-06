using System.Text.RegularExpressions;

namespace Hs.LangSense;

public enum TokenKind { Word, Number, Email, Url, Symbol, Whitespace, PersianWord, ArabicWord, LatinWord }
public sealed record ValueToken(string Text, int Start, int Length, TokenKind Kind, ValueType ValueType, double Confidence);

public static class SmartTokenizer
{
    private static readonly Regex TokenRegex = new(@"https?://[^\s]+|[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}|[0-9۰-۹٠-٩]+(?:[.,٬٫][0-9۰-۹٠-٩]+)?|[\p{L}]+|\s+|[^\p{L}\p{N}\s]",RegexOptions.Compiled);
    public static IReadOnlyList<ValueToken> Tokenize(string? text)
    {
        var s=text??string.Empty; var list=new List<ValueToken>();
        foreach(Match m in TokenRegex.Matches(s))
        {
            var v=m.Value; TokenKind kind; ValueType type; double confidence;
            if(v.Any(char.IsWhiteSpace)){kind=TokenKind.Whitespace;type=ValueType.Text;confidence=1;}
            else if(EmailValue.IsValid(v)){kind=TokenKind.Email;type=ValueType.Email;confidence=.99;}
            else if(UrlValue.IsValid(v)){kind=TokenKind.Url;type=ValueType.Url;confidence=.99;}
            else if(v.All(char.IsDigit)||v.Any(c=>c>='۰'&&c<='۹')||v.Any(c=>c>='٠'&&c<='٩')){kind=TokenKind.Number;type=ValueType.Number;confidence=.9;}
            else if(v.All(char.IsLetter)){var lang=LanguageAnalyzer.Detect(v);kind=lang==Language.Persian?TokenKind.PersianWord:lang==Language.Arabic?TokenKind.ArabicWord:TokenKind.LatinWord;type=ValueType.Text;confidence=.8;}
            else {kind=TokenKind.Symbol;type=ValueType.Text;confidence=.7;}
            list.Add(new(v,m.Index,m.Length,kind,type,confidence));
        }
        return list;
    }
}
