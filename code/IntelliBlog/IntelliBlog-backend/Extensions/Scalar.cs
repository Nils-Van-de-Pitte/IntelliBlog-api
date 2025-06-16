using Scalar.AspNetCore;

namespace IntelliBlog_backend.Extensions;

public static class Scalar
{
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