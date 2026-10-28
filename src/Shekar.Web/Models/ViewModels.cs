using Shekar.Application.Contracts;

namespace Shekar.Web.Models;

public sealed record HuntsViewModel(
    HuntListDto List,
    IReadOnlyList<CategorySummaryDto> Categories,
    SubscriptionStatusDto Status,
    DateTimeOffset Now);

public sealed record HuntDetailViewModel(HuntDetailDto Detail, DateTimeOffset Now);

public sealed record CategoriesViewModel(
    IReadOnlyList<CategorySummaryDto> Categories,
    int TodayTotal,
    SubscriptionStatusDto Status);

public sealed record AppraiseViewModel(AppraisalQuotaDto Quota);

public sealed record LoginOtpViewModel(string Mobile);

public sealed record LoginProfileViewModel(IReadOnlyList<(int Id, string Name, bool IsActive)> Cities);

public sealed record PlansViewModel(
    IReadOnlyList<PlanDto> Plans,
    CityCapacityDto Capacity);

public sealed record AccountViewModel(
    SubscriptionStatusDto Status,
    string FullName,
    string Mobile,
    string CityName,
    string ExpiresLabel,
    int DaysRemaining,
    IReadOnlyList<BellToggleResultDto> Bells,
    IReadOnlyList<AppraisalHistoryItemDto> Appraisals,
    IReadOnlyList<CategorySummaryDto> Categories);
