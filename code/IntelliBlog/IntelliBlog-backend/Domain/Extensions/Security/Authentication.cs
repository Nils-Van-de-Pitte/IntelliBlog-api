using FastEndpoints.Security;
using IntelliBlog_backend.Infrastructure.Middleware;

namespace IntelliBlog_backend.Domain.Extensions.Security;

public static class Authentication
{
    /// <summary>
    /// Adds authentication and authorization services to the application.
    /// </summary>
    /// <param name="services">The service collection to which authentication and authorization services will be added.</param>
    /// <param name="configuration">The configuration interface to retrieve authentication settings like the JWT secret.</param>
    public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationJwtBearer(s =>
            s.SigningKey = configuration["JWT_SECRET"]
        );
        
        services.AddAuthorizationBuilder()
            .AddPolicy("Users", policy => policy.RequireRole("user"))
            .AddPolicy("Admins", policy => policy.RequireRole("admin"));
    }

    /// <summary>
    /// Configures the application to use authentication and authorization middleware.
    /// </summary>
    /// <param name="app">The web application to configure with security middleware.</param>
    public static void UseSecurity(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
        app.UseMiddleware<JwtFromCookieMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

    }
}