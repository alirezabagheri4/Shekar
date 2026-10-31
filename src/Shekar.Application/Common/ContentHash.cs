using System.Security.Cryptography;
using System.Text;

namespace Shekar.Application.Common;

/// <summary>اثرانگشت محتوای آگهی برای تشخیص آگهی تکراریِ نزدیک.</summary>
public static class ContentHash
{
    /// <summary>از شماره + عنوانِ نرمال‌شده + قیمت یک هش پایدار می‌سازد.</summary>
    public static string Compute(string? phone, string title, long priceToman)
    {
        string normalizedTitle = new string((title ?? string.Empty)
            .Trim()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray())
            .ToLowerInvariant();

        string raw = $"{phone}|{normalizedTitle}|{priceToman}";
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }
}
