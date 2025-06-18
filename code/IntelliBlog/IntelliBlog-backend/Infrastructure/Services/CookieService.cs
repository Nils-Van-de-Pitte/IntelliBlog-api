using IntelliBlog_backend.Domain.Interfaces;

namespace IntelliBlog_backend.Infrastructure.Services;

public sealed class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public void SetCookie(string key, string value, int? expireDays = 1)
    {
        var options = new CookieOptions
        {
            Expires = expireDays.HasValue ? DateTime.Now.AddDays(expireDays.Value) : DateTime.Now.AddDays(expireDays!.Value),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        
        _httpContextAccessor.HttpContext!.Response.Cookies.Append(key, value, options);
    }

    public string? GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext!.Request.Cookies[key];
    }

    public void DeleteCookie(string key)
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(key);
    }
}