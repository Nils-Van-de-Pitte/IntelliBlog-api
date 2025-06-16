namespace IntelliBlog_backend.Extensions;

public static class Dependencies
{
    public static void UseDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        // Add all dependencies here
    }
}