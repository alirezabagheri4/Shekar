using Shekar.Application.Contracts;

namespace Shekar.Application.Abstractions;

/// <summary>
/// موتور تحلیل آگهی (در MVP ماک). نتیجه‌ی کارشناسی یک لینک معتبر را تولید می‌کند.
/// قوانین سهمیه/دسترسی در سرویس اعمال می‌شوند؛ این فقط تحلیل را می‌سازد.
/// </summary>
public interface IAppraisalEngine
{
    AppraisalResultDto Analyze(string adUrl);
}
