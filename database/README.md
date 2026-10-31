# دیتابیس شکار

طراحی دیتابیس با **EF Core 6** (پروژه‌ی `Shekar.Infrastructure`) و اسکریپت معادل `schema.sql`.

## سوییچ منبع داده
در `appsettings.json`:
- `"Persistence": "Mock"` → داده‌ی درون‌حافظه (پیش‌فرض، بدون دیتابیس).
- `"Persistence": "SqlServer"` → EF Core 6 روی SQL Server؛ `ConnectionStrings:Shekar` را تنظیم کنید.

## ساخت دیتابیس
دو راه:
1. **اسکریپت دستی:** فایل `database/schema.sql` را روی یک دیتابیس خالی اجرا کنید (جداول + ایندکس + کلید خارجی + داده‌ی اولیه‌ی Cities/Plans/PlanLimits/SystemSettings).
2. **EF Migrations:** چون هر ستون در Configurationها `HasComment` دارد، توضیحات ستون‌ها هم در دیتابیس ساخته می‌شود:
   ```bash
   dotnet ef migrations add Init -p src/Shekar.Infrastructure -s src/Shekar.Web
   dotnet ef database update -p src/Shekar.Infrastructure -s src/Shekar.Web
   ```

## جدول‌ها
| جدول | کاربرد |
|---|---|
| Cities | شهرها + ظرفیت (R1) |
| Plans / PlanLimits | پلن‌ها و **محدودیت‌های هر پلن** (کارشناسی، خبرم‌کن، تریال، …) |
| Users / Subscriptions | کاربران و اشتراک‌ها (Trial+Active در ظرفیت) |
| Hunts / HuntAnalyses / HuntAnalysisFactors | پیشنهادهای روزانه و تحلیل جزئیات |
| Appraisals / BellAlerts / WaitlistEntries / OtpCodes | کارشناسی، خبرم‌کن، لیست انتظار، OTP |
| Advertisers | حجم آگهی‌گذاری هر شماره (تشخیص دلال/اسپم) |
| **BlockedAdvertisers** | **شماره‌های بلاک‌شده؛ آگهی‌هایشان از پیشنهادهای روزانه حذف می‌شود** |
| **SystemSettings** | **محدودیت‌ها و تنظیمات سراسری** (کلید/مقدار) |

حذف شماره‌های بلاک‌شده از لیست روزانه، هم در EF (`EfHuntRepository`) و هم در Mock (`MockHuntRepository`) اعمال می‌شود؛ نمونه‌ی کوئری در انتهای `schema.sql`.
