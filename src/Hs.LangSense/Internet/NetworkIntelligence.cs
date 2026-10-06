using System.Net;
using System.Text.RegularExpressions;

namespace Hs.LangSense;

public static class DomainValue
{
    public static bool IsValid(string? value)
    {
        var s = value?.Trim() ?? string.Empty;
        return Regex.IsMatch(s, @"^(?=.{1,253}$)(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,63}$");
    }

    public static string Normalize(string value) => value.Trim().TrimEnd('.').ToLowerInvariant();
}

public static class NetworkAddress
{
    public static ValueType Detect(string? value)
    {
        if (IPAddress.TryParse(value, out var ip))
            return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? ValueType.IPv4 : ValueType.IPv6;
        if (DomainValue.IsValid(value)) return ValueType.Domain;
        return ValueType.Unknown;
    }
}
