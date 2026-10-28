using Shekar.Application.Abstractions;
using Shekar.Domain;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Mock;

/// <summary>
/// پایگاه‌داده‌ی درون‌حافظه‌ای و singleton برای داده‌های ماک.
/// داده‌ها عیناً از صفحات نمونه (لیست شکارهای دسکتاپ، حساب، جزئیات) برداشته شده‌اند.
/// </summary>
public sealed class MockDataStore
{
    private readonly object _gate = new();

    public List<City> Cities { get; } = new();
    public List<Plan> Plans { get; } = new();
    public List<User> Users { get; } = new();
    public List<Subscription> Subscriptions { get; } = new();
    public List<Hunt> Hunts { get; } = new();
    public List<Appraisal> Appraisals { get; } = new();
    public List<BellAlert> BellAlerts { get; } = new();
    public List<WaitlistEntry> Waitlist { get; } = new();
    public List<OtpCode> Otps { get; } = new();

    /// <summary>شناسه‌ی کاربر نمونه‌ی لاگین‌شده (کامران رضایی، مشترک حرفه‌ای).</summary>
    public const int DemoUserId = 1;

    public MockDataStore(IClock clock)
    {
        Seed(clock.UtcNow);
    }

    public int NextId<T>(IEnumerable<T> items, Func<T, int> idSelector)
    {
        lock (_gate)
        {
            return items.Select(idSelector).DefaultIfEmpty(0).Max() + 1;
        }
    }

    public object Gate => _gate;

    private void Seed(DateTimeOffset now)
    {
        Cities.AddRange(new[]
        {
            new City { Id = 1, Name = "تهران", Slug = "tehran", IsActive = true, Capacity = DomainConstants.CityCapacity },
            new City { Id = 2, Name = "کرج", Slug = "karaj", IsActive = false, Capacity = DomainConstants.CityCapacity },
            new City { Id = 3, Name = "اصفهان", Slug = "isfahan", IsActive = false, Capacity = DomainConstants.CityCapacity },
            new City { Id = 4, Name = "مشهد", Slug = "mashhad", IsActive = false, Capacity = DomainConstants.CityCapacity }
        });

        Plans.Add(new Plan
        {
            Id = 1,
            Type = PlanType.Hunter,
            Name = "شکارچی",
            MonthlyPriceToman = 990_000,
            Features = new[]
            {
                "شکارها در لحظه، همه‌ی دسته‌ها",
                "تحلیل کامل هر آگهی",
                "کارشناسی لینک آگهی نامحدود"
            }
        });
        Plans.Add(new Plan
        {
            Id = 2,
            Type = PlanType.HunterPro,
            Name = "شکارچی حرفه‌ای",
            MonthlyPriceToman = 1_499_000,
            Features = new[]
            {
                "همه‌ی امکانات شکارچی",
                "پیامک شکارهای تازه‌ی هر دسته",
                "زنگوله‌ی «خبرم کن» ۲۴ ساعته",
                "اولویت در پشتیبانی"
            }
        });

        Users.Add(new User
        {
            Id = DemoUserId,
            Mobile = "09123456789",
            FirstName = "کامران",
            LastName = "رضایی",
            CityId = 1,
            CreatedAt = now.AddMonths(-2),
            HasUsedTrial = true,
            ProfileCompleted = true
        });

        Subscriptions.Add(new Subscription
        {
            Id = 1,
            UserId = DemoUserId,
            CityId = 1,
            PlanType = PlanType.HunterPro,
            Status = SubscriptionStatus.Active,
            StartsAt = now.AddDays(-1),
            ExpiresAt = now.AddDays(30)
        });

        SeedHunts(now);
        SeedBells(now);
        SeedAppraisals(now);
    }

    private void SeedHunts(DateTimeOffset now)
    {
        int id = 1;
        void Add(string title, string year, int? km, string loc, long price, long market,
            int below, int score, Liquidity liq, double hoursAgo, bool isNew, string? img,
            HuntState state = HuntState.Active, HuntAnalysis? analysis = null)
        {
            Hunts.Add(new Hunt
            {
                Id = id++,
                Title = title,
                ModelYear = year,
                MileageKm = km,
                Location = loc,
                CityId = 1,
                PriceToman = price,
                MarketMedianToman = market,
                BelowMarketPercent = below,
                Score = score,
                Liquidity = liq,
                State = state,
                PublishedAt = now.AddHours(-hoursAgo),
                IsNew = isNew,
                ImageFileName = img,
                Analysis = analysis
            });
        }

        Add("آریسان دوگانه‌سوز شرکتی ۱۴۰۴ صفر", "۱۴۰۴", null, "پیروزی", 985_000_000, 1_200_000_000, 18, 87, Liquidity.High, 10.0 / 60, true, null);
        Add("پژو ۲۰۷ دنده‌ای پانوراما ۱۴۰۲", "۱۴۰۲", 28_000, "تهرانپارس", 1_340_000_000, 1_560_000_000, 14, 87, Liquidity.High, 35.0 / 60, true, "peugeot-207.webp", analysis: Peugeot207Analysis());
        Add("ام‌وی‌ام X55 پرو اکسلنت ۱۴۰۳", "۱۴۰۳", 11_000, "ستارخان", 2_980_000_000, 3_430_000_000, 13, 81, Liquidity.High, 2, false, "mvm-x55.webp");
        Add("چانگان یونی‌وی ۱۴۰۳", "۱۴۰۳", 6_000, "سعادت‌آباد", 4_600_000_000, 5_230_000_000, 12, 80, Liquidity.High, 1, false, "changan-univ.webp");
        Add("تارا اتوماتیک V4 ۱۴۰۳", "۱۴۰۳", 15_000, "پونک", 1_890_000_000, 2_150_000_000, 12, 79, Liquidity.High, 2, false, "tara.webp");
        Add("کوییک کراس اتوماتیک ۱۴۰۲", "۱۴۰۲", 24_000, "تهرانسر", 945_000_000, 1_070_000_000, 12, 79, Liquidity.High, 3, false, "quick-cross.webp");
        Add("نیسان N7 برقی ۲۰۲۵", "۲۰۲۵", 3_000, "شهرک غرب", 7_200_000_000, 7_910_000_000, 9, 78, Liquidity.Medium, 7, false, "nissan-n7.webp");
        Add("کیا سراتو ۲۰۰۰ مدل ۲۰۱۷", "۲۰۱۷", 96_000, "یوسف‌آباد", 3_400_000_000, 3_820_000_000, 11, 77, Liquidity.High, 4, false, "kia-cerato.webp");
        Add("دنا پلاس توربو اتوماتیک ۱۴۰۲", "۱۴۰۲", 42_000, "نارمک", 2_450_000_000, 2_750_000_000, 11, 76, Liquidity.High, 3, false, "dena-plus.webp");
        Add("بی‌ام‌و ۷۳۰Li ام‌پکیج ۲۰۱۷", "۲۰۱۷", 68_000, "زعفرانیه", 14_500_000_000, 16_290_000_000, 11, 75, Liquidity.Medium, 8, false, "bmw-730li.webp");
        Add("فونیکس آریزو ۶ پرو ۱۴۰۲", "۱۴۰۲", 38_000, "جنت‌آباد", 2_150_000_000, 2_360_000_000, 9, 74, Liquidity.Medium, 4, false, "arrizo-6-pro.webp");
        Add("هیوندای سوناتا LF مدل ۲۰۱۶", "۲۰۱۶", 110_000, "پاسداران", 3_850_000_000, 4_230_000_000, 9, 74, Liquidity.Medium, 5, false, "hyundai-sonata.webp");
        Add("تویوتا لندکروزر پرادو ۲۰۲۵", "۲۰۲۵", null, "فرمانیه", 26_500_000_000, 29_120_000_000, 9, 73, Liquidity.Medium, 7, false, "toyota-prado.webp");
        Add("تویوتا کمری هیبرید ۲۰۱۸", "۲۰۱۸", 76_000, "ونک", 6_900_000_000, 7_500_000_000, 8, 71, Liquidity.Medium, 5, false, null, HuntState.Removed);
        Add("بنز E250 مدل ۲۰۱۷", "۲۰۱۷", 85_000, "الهیه", 19_800_000_000, 21_760_000_000, 9, 70, Liquidity.Medium, 10, false, null);
    }

    private static HuntAnalysis Peugeot207Analysis() => new()
    {
        ComparableCount = 42,
        Confidence = "بالا",
        FairRangeMinToman = 1_500_000_000,
        FairRangeMaxToman = 1_620_000_000,
        Summary = "قیمت پایین + فروش سریع؛ یعنی حاشیه‌ی سود خوب با ریسک کم.",
        Factors = new[]
        {
            new AnalysisFactor("قیمت", "عالی", "۱۴٪ زیر میانه‌ی بازار"),
            new AnalysisFactor("نقدشوندگی", "عالی", "۲۰۷های مشابه معمولاً ۲ تا ۴ روزه فروش می‌رن"),
            new AnalysisFactor("کارکرد", "خیلی خوب", "۲۸ هزار کیلومتر؛ کمتر از میانگین این مدل"),
            new AnalysisFactor("وضعیت فنی", "خیلی خوب", "طبق آگهی: یه لکه رنگ گلگیر جلو"),
            new AnalysisFactor("کم‌ریسکی", "خوب", "۲ نکته قبل از خرید چک کن")
        },
        Risks = new[] { "رنگ‌شدگی ذکر شده", "بیمه‌ی معتبر", "نقدشوندگی بالا" }
    };

    private void SeedBells(DateTimeOffset now)
    {
        // حساب نمونه: دو زنگوله‌ی روشن (۱–۱٫۵ب و ۱٫۵–۲ب)، بقیه خاموش.
        BellAlerts.Add(new BellAlert
        {
            Id = 1,
            UserId = DemoUserId,
            Category = PriceCategory.From1To1_5B,
            ActivatedAt = now.AddHours(-3),
            ExpiresAt = now.AddHours(21)
        });
        BellAlerts.Add(new BellAlert
        {
            Id = 2,
            UserId = DemoUserId,
            Category = PriceCategory.From1_5To2B,
            ActivatedAt = now.AddHours(-16),
            ExpiresAt = now.AddHours(8)
        });
    }

    private void SeedAppraisals(DateTimeOffset now)
    {
        Appraisals.Add(new Appraisal { Id = 1, UserId = DemoUserId, AdUrl = "https://example.com/ad/207", Title = "پژو ۲۰۷ پانوراما ۱۴۰۲", Score = 87, Verdict = "فرصت طلایی", CreatedAt = now.AddDays(-5) });
        Appraisals.Add(new Appraisal { Id = 2, UserId = DemoUserId, AdUrl = "https://example.com/ad/tara", Title = "تارا اتوماتیک ۱۴۰۳", Score = 64, Verdict = "منصفانه", CreatedAt = now.AddDays(-8) });
        Appraisals.Add(new Appraisal { Id = 3, UserId = DemoUserId, AdUrl = "https://example.com/ad/dena", Title = "دنا پلاس ۱۴۰۱", Score = 38, Verdict = "گران", CreatedAt = now.AddDays(-14) });
    }
}
