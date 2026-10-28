using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Shekar.Application.Common;

namespace Shekar.Web.Infrastructure;

/// <summary>خطاهای کسب‌وکار را به ProblemDetails با Status Code استاندارد نگاشت می‌کند.</summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        (int status, string title, string? code) = Map(exception);

        if (status >= 500)
        {
            _logger.LogError(exception, "خطای مدیریت‌نشده در پردازش درخواست");
        }

        context.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception is AppException ? exception.Message : null
        };
        if (code is not null)
        {
            problem.Extensions["code"] = code;
        }

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = problem
        });
    }

    private static (int Status, string Title, string? Code) Map(Exception exception) => exception switch
    {
        NotAuthenticatedException e => (StatusCodes.Status401Unauthorized, "ابتدا وارد شو", e.Code),
        ForbiddenException e => (StatusCodes.Status403Forbidden, "دسترسی مجاز نیست", e.Code),
        NotFoundException e => (StatusCodes.Status404NotFound, "پیدا نشد", e.Code),
        ConflictException e => (StatusCodes.Status409Conflict, "امکان‌پذیر نیست", e.Code),
        AppValidationException e => (StatusCodes.Status400BadRequest, "ورودی نامعتبر", e.Code),
        _ => (StatusCodes.Status500InternalServerError, "خطای غیرمنتظره", null)
    };
}
