using FastEndpoints;

namespace IntelliBlog_backend.Domain.Extensions;

public static class FastEndpoints
{
    /// <summary>
    /// Configures the application to use FastEndpoints and enable response caching.
    /// </summary>
    /// <param name="app">The web application to which the configuration is applied.</param>
    public static void UseFastEndpoint(this WebApplication app)
    {
        app.UseResponseCaching()
            .UseFastEndpoints();
    }
}