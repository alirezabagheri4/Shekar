using Shekar.Application.Abstractions;
using Shekar.Application.Contracts;
using Shekar.Domain.Enums;

namespace Shekar.Mock.Providers;

/// <summary>
/// منبع آگهی ماک برای توسعه‌ی پایپ‌لاین بدون کراول واقعی.
/// هر پاس، چند آگهی تصادفی (بعضی قدیمی/بی‌عکس/تکراری) تولید می‌کند تا فیلترها آزموده شوند.
/// </summary>
public sealed class MockListingSourceProvider : IListingSourceProvider
{
    private static readonly (string Brand, string Model, string Trim)[] Catalog =
    {
        ("پژو", "۲۰۷", "دستی"), ("پژو", "۲۰۶", "تیپ ۵"), ("سایپا", "پراید", "۱۱۱"),
        ("سایپا", "کوییک", "اتوماتیک"), ("ایران‌خودرو", "دنا", "پلاس توربو"),
        ("ایران‌خودرو", "تارا", "اتوماتیک"), ("ام‌وی‌ام", "X55", "پرو"), ("کیا", "سراتو", "۲۰۰۰")
    };
    private static readonly string[] Areas = { "تهرانپارس", "پونک", "سعادت‌آباد", "نارمک", "ستارخان", "یوسف‌آباد" };
    private static readonly string[] Colors = { "سفید", "مشکی", "خاکستری", "نقره‌ای" };

    private readonly IClock _clock;
    private int _seq;

    public MockListingSourceProvider(IClock clock) => _clock = clock;

    public ListingSource Source => ListingSource.Divar;

    public Task<IReadOnlyList<RawListingDto>> FetchRecentAsync(int cityId, CancellationToken ct = default)
    {
        var rnd = Random.Shared;
        DateTimeOffset now = _clock.UtcNow;
        int count = rnd.Next(8, 15);
        var list = new List<RawListingDto>(count);

        for (int i = 0; i < count; i++)
        {
            var (brand, model, trim) = Catalog[rnd.Next(Catalog.Length)];
            int year = rnd.Next(1395, 1404);
            int mileage = rnd.Next(0, 160) * 1000;
            long price = (long)rnd.Next(700, 5000) * 1_000_000;

            // عمداً چند آگهی «ناقص» تولید می‌کنیم تا فیلترها دیده شوند:
            bool oldOne = i % 7 == 0;      // قدیمی‌تر از ۲۴ ساعت
            bool noPhoto = i % 9 == 0;     // بدون عکس
            string externalId = $"MK-{cityId}-{now:yyyyMMddHH}-{++_seq}";

            list.Add(new RawListingDto
            {
                Source = Source,
                ExternalId = externalId,
                Url = $"https://divar.ir/v/{externalId}",
                Title = $"{brand} {model} {trim} مدل {year}",
                Description = "خودرو تمیز، بیمه معتبر.",
                RawBrand = brand,
                RawModel = model,
                RawTrim = trim,
                ModelYear = year,
                MileageKm = mileage == 0 ? null : mileage,
                Color = Colors[rnd.Next(Colors.Length)],
                BodyCondition = (BodyCondition)rnd.Next(1, 5),
                PriceToman = price,
                CityId = cityId,
                RawLocation = Areas[rnd.Next(Areas.Length)],
                AdvertiserPhone = $"0912{rnd.Next(1000000, 9999999)}",
                ImageCount = noPhoto ? 0 : rnd.Next(1, 12),
                FirstImageUrl = noPhoto ? null : $"https://img/{externalId}.jpg",
                PublishedAt = oldOne ? now.AddHours(-rnd.Next(30, 72)) : now.AddMinutes(-rnd.Next(1, 1200)),
                RawJson = null
            });
        }

        return Task.FromResult<IReadOnlyList<RawListingDto>>(list);
    }
}
