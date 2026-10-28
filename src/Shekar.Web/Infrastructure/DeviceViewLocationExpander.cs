using Microsoft.AspNetCore.Mvc.Razor;

namespace Shekar.Web.Infrastructure;

/// <summary>
/// Viewها را از پوشه‌ی دستگاه (Desktop/Mobile) انتخاب می‌کند تا یک بک‌اند دو قالب را سرو کند.
/// </summary>
public sealed class DeviceViewLocationExpander : IViewLocationExpander
{
    private const string Key = "device";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        string device = context.ActionContext.HttpContext.Items.TryGetValue("Device", out object? v)
            && v is string s ? s : nameof(DeviceKind.Desktop);
        context.Values[Key] = device;
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        string device = context.Values.TryGetValue(Key, out string? d) ? d! : nameof(DeviceKind.Desktop);

        // ابتدا پوشه‌ی دستگاه، سپس مسیرهای پیش‌فرض به‌عنوان جایگزین.
        var deviceLocations = new[]
        {
            $"/Views/{{1}}/{device}/{{0}}.cshtml",
            $"/Views/Shared/{device}/{{0}}.cshtml"
        };

        return deviceLocations.Concat(viewLocations);
    }
}
