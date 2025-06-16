using Scalar.AspNetCore;

namespace IntelliBlog_backend.Extensions;

public static class Scalar
{
    /// <summary>
    /// Configures the application to use Scalar for API reference.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    public static void UseScalar(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "IntelliBlog API Reference";
            options.Theme = ScalarTheme.Kepler;
            options.Layout = ScalarLayout.Classic;
        });

    }
}