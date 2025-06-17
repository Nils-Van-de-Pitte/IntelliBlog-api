using FastEndpoints;

namespace IntelliBlog_backend.Domain.Extensions;

public static class Routes
{
    /// <summary>
    /// Configures the application with defined routes.
    /// </summary>
    /// <param name="app">The web application to configure with routes.</param>
    public static void UseRoutes(this WebApplication app)
    {
        app.UseResponseCaching()
        .UseFastEndpoints();
    }
}