using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shekar.Domain.Entities;

namespace Shekar.Infrastructure.Persistence.Configurations;

public sealed class AppraisalConfiguration : IEntityTypeConfiguration<Appraisal>
{
    public void Configure(EntityTypeBuilder<Appraisal> b)
    {
        b.ToTable("Appraisals").HasComment("کارشناسی‌های قیمت کاربران (R5)");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasComment("کاربر انجام‌دهنده");
        b.Property(x => x.AdUrl).HasMaxLength(500).IsRequired().HasComment("لینک آگهی کارشناسی‌شده");
        b.Property(x => x.Title).HasMaxLength(200).HasComment("عنوان خودرو");
        b.Property(x => x.Score).HasComment("امتیاز نتیجه ۰ تا ۱۰۰");
        b.Property(x => x.Verdict).HasMaxLength(40).HasComment("برچسب نتیجه: «فرصت طلایی»/«منصفانه»/«گران»");
        b.Property(x => x.CreatedAt).HasComment("زمان انجام کارشناسی (مبنای شمارش سهمیه‌ی ماهانه)");
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        // ایندکس شمارش سهمیه‌ی ماهانه‌ی هر کاربر.
        b.HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}

public sealed class BellAlertConfiguration : IEntityTypeConfiguration<BellAlert>
{
    public void Configure(EntityTypeBuilder<BellAlert> b)
    {
        b.ToTable("BellAlerts").HasComment("زنگوله‌های «خبرم کن» هر کاربر برای دسته‌های قیمتی (R6)");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasComment("کاربر صاحب زنگوله");
        b.Property(x => x.Category).HasConversion<byte>().HasComment("دسته‌ی قیمتی (۱ تا ۸)");
        b.Property(x => x.ActivatedAt).HasComment("زمان روشن‌شدن؛ NULL=خاموش");
        b.Property(x => x.ExpiresAt).HasComment("زمان خاموش‌شدن خودکار (ActivatedAt + ۲۴ ساعت)");
        b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.UserId, x.Category }).IsUnique();
    }
}

public sealed class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> b)
    {
        b.ToTable("WaitlistEntries").HasComment("لیست انتظار شهرِ پرظرفیت (R1)");
        b.HasKey(x => x.Id);
        b.Property(x => x.CityId).HasComment("شهر لیست انتظار");
        b.Property(x => x.Mobile).HasMaxLength(15).IsRequired().HasComment("شماره موبایل متقاضی");
        b.Property(x => x.CreatedAt).HasComment("زمان ثبت در صف");
        b.Property(x => x.Position).HasComment("نوبت نمایش‌داده‌شده در صف");
        b.HasOne<City>().WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => new { x.CityId, x.Mobile }).IsUnique();
    }
}

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> b)
    {
        b.ToTable("OtpCodes").HasComment("کدهای یک‌بارمصرف ورود");
        b.HasKey(x => x.Id);
        b.Property(x => x.Mobile).HasMaxLength(15).IsRequired().HasComment("شماره موبایل مقصد");
        b.Property(x => x.Code).HasMaxLength(10).IsRequired().HasComment("کد ۵ رقمی (در Mock ثابت ۱۲۳۴۵)");
        b.Property(x => x.CreatedAt).HasComment("زمان تولید");
        b.Property(x => x.ExpiresAt).HasComment("زمان انقضا");
        b.Property(x => x.Consumed).HasComment("آیا قبلاً استفاده/تأیید شده");
        b.HasIndex(x => new { x.Mobile, x.CreatedAt });
    }
}
