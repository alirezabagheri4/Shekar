using System.Text.RegularExpressions;

namespace Shekar.Web.Infrastructure;

public enum DeviceKind
{
    Desktop,
    Mobile
}

/// <summary>نوع دستگاه درخواست جاری (از روی User-Agent).</summary>
public interface IDeviceContext
{
    DeviceKind Device { get; }
    bool IsMobile { get; }
}

public sealed class DeviceContext : IDeviceContext
{
    public DeviceKind Device { get; set; } = DeviceKind.Desktop;
    public bool IsMobile => Device == DeviceKind.Mobile;
}

/// <summary>میان‌افزار تشخیص موبایل/دسکتاپ بر پایه‌ی User-Agent (تشخیص سمت سرور).</summary>
public sealed class DeviceDetectionMiddleware
{
    private static readonly Regex MobileAgent = new(
        "Android|webOS|iPhone|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|Windows Phone",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly RequestDelegate _next;

    public DeviceDetectionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context, DeviceContext device)
    {
        string ua = context.Request.Headers.UserAgent.ToString();
        bool isTablet = ua.Contains("iPad", StringComparison.OrdinalIgnoreCase);
        device.Device = !isTablet && MobileAgent.IsMatch(ua) ? DeviceKind.Mobile : DeviceKind.Desktop;

        // امکان بازنویسی دستی برای تست: ?device=mobile|desktop
        string? forced = context.Request.Query["device"];
        if (string.Equals(forced, "mobile", StringComparison.OrdinalIgnoreCase))
        {
            device.Device = DeviceKind.Mobile;
        }
        else if (string.Equals(forced, "desktop", StringComparison.OrdinalIgnoreCase))
        {
            device.Device = DeviceKind.Desktop;
        }

        context.Items["Device"] = device.Device.ToString();
        await _next(context);
    }
}
