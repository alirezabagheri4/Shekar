using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Shekar.Application;
using Shekar.Application.Abstractions;
using Shekar.Mock;
using Shekar.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// UI (MVC Views) + API Controllers در یک Host واحد.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// تشخیص دستگاه + انتخاب View بر اساس دستگاه (دو قالب، یک بک‌اند).
builder.Services.AddScoped<DeviceContext>();
builder.Services.AddScoped<IDeviceContext>(sp => sp.GetRequiredService<DeviceContext>());
builder.Services.Configure<RazorViewEngineOptions>(options =>
    options.ViewLocationExpanders.Add(new DeviceViewLocationExpander()));

// لایه‌های برنامه؛ پیش‌فرض Mock.
builder.Services.AddApplicationServices();
builder.Services.AddMockInfrastructure();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ProblemDetails + Exception Handler سراسری.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// احراز هویت Cookie.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "shekar.auth";
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<DeviceDetectionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
