# شکار — فایل‌های نهایی UI

- `desktop/`: ۱۷ صفحه‌ی دسکتاپ (۱۴۴۰px)
- `mobile/`: ۱۸ صفحه‌ی موبایل (تا ۴۸۰px)
- `*/css/base.css`: استایل مشترک صفحات داخلی (توکن‌ها، دکمه‌ها، انیمیشن‌های ضروری، فوکوس)
- `*/css/<صفحه>.css`: استایل اختصاصی هر صفحه. `index.css` (لندینگ) مستقل است.
- `assets/`: لوگو، تصاویر دسته‌ها و عکس آگهی‌ها

| فایل | صفحه |
|---|---|
| index | لندینگ |
| login · login-otp · login-profile | ورود (۳ مرحله) |
| plans · plans-city-full | پلن‌ها · ظرفیت تکمیل و لیست انتظار |
| pay · pay-success | درگاه · پرداخت موفق |
| categories | انتخاب دسته (فقط موبایل) |
| hunts · hunts-locked | لیست شکارها · حالت قفل |
| hunt-detail | جزئیات شکار |
| appraise · appraise-quota-exhausted | کارشناسی · اتمام سهمیه |
| analyze · analyze-result | در حال تحلیل · نتیجه |
| account · contact | حساب · تماس |

بدون JavaScript؛ لینک‌های بین صفحات کار می‌کنند. فونت Vazirmatn از Google Fonts لود می‌شود.
