namespace IntelliBlog_backend.Domain.Extensions.Security;

public static class Cors
{
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                name: "CorsPolicy",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000");
                    policy.AllowCredentials();
                }
                );
        });
    }
}