using System.Net;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public sealed record EmailValue(string Address, string Username, string Domain)
{
    public bool Valid => ValidateAddress(Address);
    public static EmailValue Parse(string value)
    {
        if (!TryParse(value, out var result)) throw new FormatException("Invalid email address.");
        return result;
    }
    public static bool TryParse(string? value, out EmailValue result)
    {
        var s = (value ?? string.Empty).Trim();
        var m = Regex.Match(s, @"^(?<u>[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+)@(?<d>[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?)+)$");
        result = m.Success ? new EmailValue(s,m.Groups["u"].Value,m.Groups["d"].Value.ToLowerInvariant()) : new EmailValue(s,string.Empty,string.Empty);
        return m.Success;
    }
    public static bool IsValid(string? value) => TryParse(value, out _);
    private static bool ValidateAddress(string value) => TryParse(value, out _);
}

public sealed record UrlValue(Uri Uri)
{
    public string Value => Uri.ToString();
    public string Host => Uri.Host;
    public static bool TryParse(string? value, out UrlValue result)
    {
        result = null!;
        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Host)) return false;
        result = new UrlValue(uri); return true;
    }
    public static bool IsValid(string? value) => TryParse(value, out _);
}

public sealed record IpValue(IPAddress Address)
{
    public string Value => Address.ToString();
    public bool IsV4 => Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
    public bool IsV6 => Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
    public static bool TryParse(string? value, out IpValue result)
    {
        result = null!;
        if (!IPAddress.TryParse(value?.Trim(), out var ip)) return false;
        result = new IpValue(ip); return true;
    }
    public static bool IsValid(string? value) => TryParse(value, out _);
}
