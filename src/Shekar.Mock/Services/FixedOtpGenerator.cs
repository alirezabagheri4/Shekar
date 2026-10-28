using Shekar.Application.Abstractions;

namespace Shekar.Mock.Services;

/// <summary>در Mock کد OTP همیشه «۱۲۳۴۵» است.</summary>
public sealed class FixedOtpGenerator : IOtpGenerator
{
    public const string FixedCode = "12345";

    public string Generate() => FixedCode;
}
