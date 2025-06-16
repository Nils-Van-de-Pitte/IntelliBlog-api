using FastEndpoints;

namespace IntelliBlog_backend.Extensions;

public static class Dependencies
{
    /// <summary>
    /// Configures the service collection with required dependencies for the application.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration"></param>
    public static void UseDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddFastEndpoints();
        services.AddBloggingContext(configuration);
    }
}