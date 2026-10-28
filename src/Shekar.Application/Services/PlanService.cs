using Shekar.Application.Abstractions;
using Shekar.Application.Contracts;
using Shekar.Domain.Entities;
using Shekar.Domain.Enums;

namespace Shekar.Application.Services;

/// <summary>پلن‌ها برای نمایش (R2 — فقط ماهانه).</summary>
public sealed class PlanService : IPlanService
{
    private readonly IPlanRepository _plans;

    public PlanService(IPlanRepository plans)
    {
        _plans = plans;
    }

    public async Task<IReadOnlyList<PlanDto>> ListAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Plan> plans = await _plans.ListAsync(ct);
        return plans.Select(p => new PlanDto
        {
            Type = p.Type,
            Name = p.Name,
            MonthlyPriceToman = p.MonthlyPriceToman,
            Features = p.Features,
            HasBellAlerts = p.HasBellAlerts,
            IsRecommended = p.Type == PlanType.HunterPro
        }).ToList();
    }
}
