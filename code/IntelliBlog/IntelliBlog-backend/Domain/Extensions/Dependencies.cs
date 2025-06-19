using FastEndpoints;
using IntelliBlog_backend.Domain.Extensions.Security;
using IntelliBlog_backend.Domain.Interfaces;
using IntelliBlog_backend.Infrastructure.Security;
using IntelliBlog_backend.Infrastructure.Services;

namespace IntelliBlog_backend.Domain.Extensions;

public static class Dependencies
{
    /// <summary>
    /// Adds and configures dependencies for the application.
    /// </summary>
    /// <param name="services">The service collection to which the dependencies will be added.</param>
    /// <param name="configuration">The configuration interface to retrieve configuration settings.</param>
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSecurity();
        services.AddFastEndpoints();
        services.AddBloggingContext(configuration);
    }

    /// <summary>
    /// Registers application-specific services into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the services will be registered.</param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICookieService, CookieService>();
    }
}