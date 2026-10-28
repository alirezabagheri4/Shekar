namespace Shekar.Application.Abstractions;

/// <summary>R5 — بررسی اینکه لینک، URL معتبرِ یک آگهی خودرو است.</summary>
public interface IAdLinkValidator
{
    bool IsValidCarAdUrl(string? url);
}
