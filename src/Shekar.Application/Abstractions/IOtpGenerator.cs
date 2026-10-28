namespace Shekar.Application.Abstractions;

/// <summary>تولید کد OTP. در Mock همیشه «۱۲۳۴۵»؛ در تولید تصادفی.</summary>
public interface IOtpGenerator
{
    string Generate();
}
