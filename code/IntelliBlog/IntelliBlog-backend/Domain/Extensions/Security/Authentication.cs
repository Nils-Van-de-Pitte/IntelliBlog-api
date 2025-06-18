using FastEndpoints.Security;

namespace IntelliBlog_backend.Domain.Extensions.Security;

public static class Authentication
{
    /// <summary>
    /// Adds security configurations, including authentication and authorization, to the application.
    /// </summary>
    /// <param name="services">The service collection to which the security configurations will be added.</param>
    public static void AddSecurity(this IServiceCollection services)
    {
        services.AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(10));
        services.AddAuthenticationJwtBearer(s => s.SigningKey = "The secret used to sign the JWT");
        services.AddAuthorizationBuilder()
            .AddPolicy("Users", policy => policy.RequireRole("user"))
            .AddPolicy("Admin", policy => policy.RequireRole("admin"));
        
        
    }

    /// <summary>
    /// Configures the application to use authentication and authorization middleware.
    /// </summary>
    /// <param name="app">The web application to configure with security middleware.</param>
    public static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}