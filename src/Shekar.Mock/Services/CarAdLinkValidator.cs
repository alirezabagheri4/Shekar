using Shekar.Application.Abstractions;

namespace Shekar.Mock.Services;

/// <summary>R5 — لینک باید یک URL مطلقِ http/https باشد.</summary>
public sealed class CarAdLinkValidator : IAdLinkValidator
{
    public bool IsValidCarAdUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return Uri.TryCreate(url.Trim(), UriKind.Absolute, out Uri? uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && !string.IsNullOrEmpty(uri.Host)
            && uri.Host.Contains('.');
    }
}
