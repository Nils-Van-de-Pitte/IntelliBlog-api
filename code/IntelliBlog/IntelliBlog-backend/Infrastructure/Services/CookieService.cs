using IntelliBlog_backend.Domain.Interfaces;

namespace IntelliBlog_backend.Infrastructure.Services;

public sealed class CookieService(IHttpContextAccessor httpContextAccessor) : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Sets a cookie in the HTTP response with the specified key, value, and expiration time.
    /// </summary>
    /// <param name="key">The key of the cookie to set.</param>
    /// <param name="value">The value of the cookie to set.</param>
    /// <param name="expireDays">The number of days until the cookie expires. Defaults to 1 day.</param>
    public void SetCookie(string key, string value, int expireDays = 1)
    {
        var options = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(expireDays),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        
        _httpContextAccessor.HttpContext!.Response.Cookies.Append(key, value, options);
    }

    /// <summary>
    /// Retrieves the value of a specified cookie by its key from the current HTTP request.
    /// </summary>
    /// <param name="key">The key of the cookie to retrieve.</param>
    /// <returns>The value of the cookie if found; otherwise, null.</returns>
    public string? GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext!.Request.Cookies[key];
    }

    /// <summary>
    /// Deletes a cookie with the specified key from the HTTP response.
    /// </summary>
    /// <param name="key">The key of the cookie to be deleted.</param>
    public void DeleteCookie(string key)
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete(key);
    }
}