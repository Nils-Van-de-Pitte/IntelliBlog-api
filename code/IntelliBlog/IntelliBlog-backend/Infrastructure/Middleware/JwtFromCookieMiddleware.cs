namespace IntelliBlog_backend.Infrastructure.Middleware;

public class JwtFromCookieMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Only add an authorization header if it doesn't already exist
        if (!context.Request.Headers.ContainsKey("Authorization") &&
            context.Request.Cookies.TryGetValue("token", out var token) &&
            !string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";
        }

        await _next(context);
    }

}