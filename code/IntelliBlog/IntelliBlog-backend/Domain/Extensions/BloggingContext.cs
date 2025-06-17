using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Domain.Extensions;

public static class BloggingContextExtension
{
    public static void AddBloggingContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BloggingContext>(options =>
        {
            var host = configuration["DB_HOST"];
            var name = configuration["DB_NAME"];
            var user = configuration["DB_USER"];
            var password = configuration["DB_PASSWORD"];
            var port = configuration["DB_PORT"];

            var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={name};";
            options.UseNpgsql(connectionString);
        });
    }
}