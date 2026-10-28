# شکار — نقشه‌ی توسعه (MVP · .NET 8)

منبع حقیقت UI: `design-reference/` (۱۷ صفحه‌ی دسکتاپ ۱۴۴۰px + ۱۸ صفحه‌ی موبایل تا ۴۸۰px، CSS و assets).

---

## توکن‌های دیزاین (استخراج‌شده از `*/css/base.css`)

| گروه | مقدار |
|---|---|
| teal | `#0F766E` · 700 `#0B5D57` · 900 `#134E4A` · 50 `#E6F4F1` · 100 `#CDEBE5` |
| mint / gold / amber | `#5EEAD4` / `#D4A017` (gold-50 `#FBF6E6`) / `#D97706` (amber-50 `#FEF3C7`) |
| ink / ink-2 / muted | `#111827` / `#374151` / `#6B7280` |
| line / line-2 / soft | `#E5E7EB` / `#EEF0F2` / `#F6F8F8` |
| danger | `#DC2626` (danger-50 `#FEE2E2`) |
| radius | sm 8 · md 14 · lg 20 · xl 28 |
| shadow | sh-1 `0 2px 8px rgba(17,24,39,.06)` · sh-2 `0 12px 32px rgba(15,60,55,.10)` · sh-3 `0 24px 56px rgba(15,60,55,.16)` |
| دکمه | `.zbtn` min-height 52px, radius md, font 16/700؛ p/s/g/w/off |
| فوکوس | outline 3px `--mint` offset 2 |
| انیمیشن | zpulse, zsh(shimmer), zspin, zbar, zpop, zscan, zgrow, zring + `prefers-reduced-motion` |
| فونت | Vazirmatn 400–800 (در نمونه Google Fonts؛ **در پروژه self-host woff2 + font-display:swap**) |

کلاس‌های کلیدی: `zcard`, `zcard-lock`, `zcard-off`, `zcat`, `zbell`/`zbell-on`/`zbell-lock`, `zsort`, `ztab`, `znav`(موبایل), `zside`, `zv`(چیپ زیربازار), `ring`(امتیاز).

---

## معماری (طبق خواسته)

```
src/
  Shekar.Domain          Entities + Enums (بدون وابستگی)
  Shekar.Application     Interfaces, DTOs, Services (کل قوانین کسب‌وکار اینجا)
  Shekar.Infrastructure  EF Core + Repository واقعی (فعلاً خالی/اسکلت)
  Shekar.Mock            پیاده‌سازی Mock همه‌ی Interfaceها + داده‌های ماک
  Shekar.Web             Razor Pages (UI دسکتاپ+موبایل) + API Controllers + wwwroot
```
یک Host واحد: `Shekar.Web` هم UI و هم API را سرو می‌کند. DI در Startup، Mock به‌صورت پیش‌فرض ثبت می‌شود (سوییچ Mock↔Infrastructure با پیکربندی).

**تصمیم‌های تأییدشده:**
- **UI**: ASP.NET Core **MVC** (Controller + Razor View)، نه Razor Pages.
- **دسکتاپ/موبایل**: تشخیص **سمت سرور با User-Agent** + دو Layout مجزا (`_LayoutDesktop` / `_LayoutMobile`)، یک URL و یک بک‌اند. میان‌افزار/سرویس `IDeviceDetector` نوع دستگاه را تعیین و View/Layout مناسب انتخاب می‌کند.

---

## تسک‌های توسعه

### فاز ۰ — پایه‌گذاری
- **T0.1** Solution + ۵ پروژه + ارجاعات + `Nullable`+`ImplicitUsings` فعال + `.editorconfig` (نام‌گذاری Microsoft).
- **T0.2** انتقال assets و فونت: دانلود Vazirmatn woff2 (400–800) → `wwwroot/fonts`؛ `site.css` با `@font-face` + `font-display:swap` (جایگزین `@import` گوگل). کپی `assets/` → `wwwroot/assets`.
- **T0.3** تبدیل `base.css` + CSSهای هر صفحه به `wwwroot/css` بدون تغییر ظاهری.

### فاز ۱ — Domain
- **T1.1** Entityها: `City`, `User`, `Subscription`, `Plan`, `Hunt`(آگهی‌شکار), `PriceCategory`, `Appraisal`, `BellAlert`, `WaitlistEntry`, `OtpCode`.
- **T1.2** Enumها: `PlanType{Hunter,HunterPro}`, `SubscriptionStatus{Trial,Active,Expired}`, `HuntLiquidity{High,Medium}`, `HuntState{Active,Removed}`, `AdValidationResult`.
- **T1.3** ثابت‌های دامنه: ۸ دسته‌ی قیمتی (میلیون تومان): `<1000 | 1000–1500 | 1500–2000 | 2000–3000 | 3000–4000 | 4000–8000 | 8000–16000 | >16000`.

### فاز ۲ — Application (قوانین + قراردادها)
- **T2.1** DTOها (ورودی/خروجی API) + DataAnnotations.
- **T2.2** Interfaceها: `IHuntService`, `IAppraisalService`, `ISubscriptionService`, `ICapacityService`, `IWaitlistService`, `IBellService`, `IAuthService`, `ISmsSender`, `IClock`(Asia/Tehran), `IJalaliCalendar`.
- **T2.3** پیاده‌سازی قوانین کسب‌وکار (همه در Application):
  - **R1 ظرفیت شهر ۱۰۰ فعال** (Trial+Active): خرید/ترایل در پر بودن → `409`؛ وضعیت `۱۰۰/۱۰۰` + صف.
  - **R2** فقط پرداخت ماهانه.
  - **R3** ۲۴ ساعت رایگان، یک‌بار برای هر کاربر تازه، روی هر پلن.
  - **R4** لیست شکار فقط مشترک؛ کاربر بدون اشتراک = حالت قفل (بنر آمار + کارت نیمه‌بلور + مودال «این شکار هنوز قفله»)؛ API جزئیات → `403`.
  - **R5** کارشناسی: مهمان→ورود؛ بدون‌اشتراک ماهی ۳ رایگان (ماه شمسی)، چهارمی `403` + «سهمیه‌ی رایگان این ماه تموم شد»؛ مشترک نامحدود؛ لینک نامعتبر → «لینک نامعتبر است!».
  - **R6** «خبرم کن» فقط پلن حرفه‌ای؛ زنگوله = ۲۴ ساعت پیامک، بعد auto-off؛ پلن پایه → مودال ارتقا.
  - **R7** آگهی حذف‌شده: کارت خاکستری «آگهی حذف شده»، بدون لینک/کلیک؛ API جزئیات → `404`.
  - **R8** امتیاز ≥ ۸۵ = «فرصت طلایی» (رنگ طلایی فقط اینجا).
  - **R9** سود خالص تخمینی = میانه‌ی بازار − قیمت آگهی − `max(۱۵م، ۲٪ قیمت)`.
  - **R10** مرزبندی ۸ دسته.
  - **R11** تب‌ها (امروز/دیروز/۲روزپیش) + مرتب‌سازی (بیشترین امتیاز/بیشترین زیربازار/جدیدترین)؛ «شکار فعال» = حذف‌نشده.
  - **R12** ساعت پشتیبانی ۰۹–۱۸ `Asia/Tehran`.
- **T2.4** تست واحد قوانین R1,R3,R5,R8,R9 (xUnit روی Application).

### فاز ۳ — Mock
- **T3.1** داده‌ی ماک از نمونه‌ها عیناً: شکارهای تهران (پژو۲۰۷ ۸۷، چانگان ۸۰، تارا ۷۹، آریسان ۸۷، X55 ۸۱، …)، دسته‌ها با شمارش، آگهی حذف‌شده (پژو۲۰۶ تیپ۵)، نمونه‌کارشناسی‌ها، کاربر `کامران رضایی`، صف انتظار.
- **T3.2** `MockSmsSender` (لاگ)، `MockAuth` کد ثابت `12345`، `MockClock`(Asia/Tehran).
- **T3.3** ثبت DI: پیش‌فرض Mock.

### فاز ۴ — Web/API (Controllerهای نازک)
- **T4.1** مسیر `api/[resource]`؛ افعال + Status استاندارد `200/201/204/400/404` + `401`(لاگین‌نشده) + `403/409`(قفل/سهمیه/ظرفیت) با ProblemDetails.
- **T4.2** Exception Handler سراسری → ProblemDetails؛ Swagger در Development.
- **T4.3** Controllerها: Hunts, HuntDetail, Appraisal, Plans/Capacity, Waitlist, Bells, Auth(OTP).
- **T4.4** فقط DTO؛ Validation با DataAnnotations؛ `async`+`CancellationToken`.

### فاز ۵ — UI (MVC Views، مطابق عین نمونه)
- **T5.1** `IDeviceDetector` (User-Agent) + دو Layout `_LayoutDesktop`/`_LayoutMobile`؛ انتخاب خودکار View/Layout بر اساس دستگاه (یک بک‌اند، دو قالب). پانویس در همه‌ی صفحات: «قیمت و سود تخمینیه و تضمینی نیست؛ قبل از خرید حتماً کارشناسی کن.»
- **T5.2** ناوبری: موبایل نوار پایین (شکارها/کارشناسی/پلن‌ها/حساب)؛ دسکتاپ هدر بالا + سایدبار دسته.
- **T5.3** ریدایرکت: `/categories` فقط موبایل؛ روی دسکتاپ (تشخیص سرور) → `/hunts`.
- **T5.4** صفحات: index، login/otp/profile (۳ مرحله)، categories(موبایل)، hunts، hunts-locked، hunt-detail، appraise، appraise-quota-exhausted، analyze، analyze-result، plans، plans-city-full(+صف)، pay، pay-success، account، contact.
- **T5.5** حالت‌های پویا از قوانین: قفل/باز، سهمیه‌ی کارشناسی، ظرفیت پر/صف، زنگوله روشن/خاموش/قفل‌پایه، آگهی حذف‌شده، فرصت طلایی.

### فاز ۶ — احراز هویت
- **T6.1** ورود موبایل+OTP ۵رقمی (از `ISmsSender`) + Cookie Auth؛ پروفایل فقط ورود اول؛ در Mock کد `12345`.

---

## ابهام‌های نمونه (باید تصمیم بگیری)
1. **موقعیت در صف** («نفر ۳۴ام صف تهرانی») در قوانین هست ولی در `plans-city-full` فقط فرم ثبت دیده می‌شود؛ متن دقیق حالت «بعد از ثبت» در نمونه نیست → پیشنهاد: «نفر Nام صف تهرانی».
2. **قیمت پلن‌ها**: شکارچی `۹۹۰,۰۰۰` و حرفه‌ای `۱,۴۹۹,۰۰۰` تومان/ماه (از index و plans). تأیید شود.
3. **مودال‌ها** («این شکار هنوز قفله»، «این آگهی حذف شده…»، ارتقای پلن پایه) در HTML استاتیک به‌صورت باز رندر نشده‌اند؛ باید مطابق لحن نمونه ساخته شوند (JS سبک یا `<dialog>`).
4. **شهر غیرتهران**: فقط تهران فعال؛ بقیه «به‌زودی» — رفتار انتخاب شهر در پروفایل برای شهر غیرفعال نامشخص است.
5. **analyze → analyze-result**: صفحه‌ی analyze انیمیشن پیشرفت دارد؛ منطق زمان‌بندی/ریدایرکت واقعی در MVP (تایمر سمت کلاینت یا سرور) باید تعیین شود.
