using Microsoft.Extensions.DependencyInjection;
using Shekar.Application.Abstractions;
using Shekar.Mock.Repositories;
using Shekar.Mock.Services;

namespace Shekar.Mock;

/// <summary>ثبت پیاده‌سازی‌های ماک (داده + زیرساخت) به‌عنوان پیش‌فرض.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddMockInfrastructure(this IServiceCollection services)
    {
        // مخزن درون‌حافظه‌ای، singleton تا داده بین درخواست‌ها بماند.
        services.AddSingleton<MockDataStore>();

        // سرویس‌های زیرساختی.
        services.AddSingleton<IClock, TehranClock>();
        services.AddSingleton<IJalaliCalendar, PersianJalaliCalendar>();
        services.AddSingleton<ISmsSender, MockSmsSender>();
        services.AddSingleton<IOtpGenerator, FixedOtpGenerator>();
        services.AddSingleton<IAdLinkValidator, CarAdLinkValidator>();
        services.AddSingleton<IAppraisalEngine, MockAppraisalEngine>();

        // Repositoryها.
        services.AddScoped<ICityRepository, MockCityRepository>();
        services.AddScoped<IPlanRepository, MockPlanRepository>();
        services.AddScoped<IUserRepository, MockUserRepository>();
        services.AddScoped<ISubscriptionRepository, MockSubscriptionRepository>();
        services.AddScoped<IHuntRepository, MockHuntRepository>();
        services.AddScoped<IAppraisalRepository, MockAppraisalRepository>();
        services.AddScoped<IBellAlertRepository, MockBellAlertRepository>();
        services.AddScoped<IWaitlistRepository, MockWaitlistRepository>();
        services.AddScoped<IOtpRepository, MockOtpRepository>();

        return services;
    }
}
