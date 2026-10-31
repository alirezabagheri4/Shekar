using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shekar.Domain.Entities;

namespace Shekar.Infrastructure.Persistence.Configurations;

public sealed class AdvertiserConfiguration : IEntityTypeConfiguration<Advertiser>
{
    public void Configure(EntityTypeBuilder<Advertiser> b)
    {
        b.ToTable("Advertisers").HasComment("آگهی‌دهنده‌ها و حجم آگهی‌گذاری‌شان؛ مبنای تشخیص دلال/اسپمر");
        b.HasKey(x => x.Id);
        b.Property(x => x.Phone).HasMaxLength(15).IsRequired().HasComment("شماره موبایل آگهی‌دهنده؛ یکتا");
        b.Property(x => x.DisplayName).HasMaxLength(80).HasComment("نام/مستعار آگهی‌دهنده");
        b.Property(x => x.FirstSeenAt).HasComment("اولین آگهی دیده‌شده از این شماره");
        b.Property(x => x.LastAdAt).HasComment("آخرین آگهی ثبت‌شده");
        b.Property(x => x.TotalAdsPosted).HasComment("مجموع کل آگهی‌های این شماره");
        b.Property(x => x.AdsPostedLast30Days).HasComment("تعداد آگهی در ۳۰ روز اخیر (تشخیص آگهی بیش از حد)");
        b.Property(x => x.IsTrusted).HasComment("آگهی‌دهنده‌ی معتبر (از فیلترها مستثنا)");
        b.HasIndex(x => x.Phone).IsUnique();
    }
}

public sealed class BlockedAdvertiserConfiguration : IEntityTypeConfiguration<BlockedAdvertiser>
{
    public void Configure(EntityTypeBuilder<BlockedAdvertiser> b)
    {
        b.ToTable("BlockedAdvertisers").HasComment("شماره‌های بلاک‌شده؛ آگهی‌هایشان از پیشنهادهای روزانه حذف می‌شود (ضد دلال/اسپم/دروغ)");
        b.HasKey(x => x.Id);
        b.Property(x => x.Phone).HasMaxLength(15).IsRequired().HasComment("شماره موبایل بلاک‌شده");
        b.Property(x => x.Reason).HasConversion<byte>().HasComment("دلیل: 1=آگهی‌زیاد،2=دلال،3=اطلاعات‌نادرست،4=تکراری،5=گزارش‌کاربر،6=دستی");
        b.Property(x => x.Note).HasMaxLength(300).HasComment("توضیح تکمیلی");
        b.Property(x => x.BlockedAt).HasComment("زمان بلاک");
        b.Property(x => x.BlockedByUserId).HasComment("ادمین بلاک‌کننده؛ NULL=سیستمی/خودکار");
        b.Property(x => x.ExpiresAt).HasComment("انقضای بلاک؛ NULL=دائمی");
        b.Property(x => x.IsActive).HasComment("آیا بلاک هم‌اکنون فعال است");
        // ایندکس تشخیص سریع بلاک فعالِ یک شماره.
        b.HasIndex(x => new { x.Phone, x.IsActive });
    }
}

public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> b)
    {
        b.ToTable("SystemSettings").HasComment("محدودیت‌ها و تنظیمات سراسری کسب‌وکار (کلید/مقدار، قابل تغییر بدون انتشار)");
        b.HasKey(x => x.Key);
        b.Property(x => x.Key).HasMaxLength(80).HasComment("کلید یکتا، مثل «GoldenScoreThreshold»");
        b.Property(x => x.Value).HasMaxLength(200).IsRequired().HasComment("مقدار (طبق ValueType تفسیر می‌شود)");
        b.Property(x => x.ValueType).HasMaxLength(20).HasComment("نوع مقدار: int|decimal|bool|time|string");
        b.Property(x => x.Description).HasMaxLength(300).HasComment("توضیح کاربردی تنظیم");
        b.Property(x => x.UpdatedAt).HasComment("زمان آخرین تغییر");
    }
}
