using Microsoft.Extensions.DependencyInjection;
using Shekar.Application.Services;

namespace Shekar.Application;

/// <summary>ثبت سرویس‌های لایه‌ی Application در DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IHuntService, HuntService>();
        services.AddScoped<IAppraisalService, AppraisalService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ICapacityService, CapacityService>();
        services.AddScoped<IWaitlistService, WaitlistService>();
        services.AddScoped<IBellService, BellService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISupportService, SupportService>();
        return services;
    }
}
