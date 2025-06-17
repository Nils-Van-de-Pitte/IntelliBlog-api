using FastEndpoints.Security;

namespace IntelliBlog_backend.Domain.Extensions.Security;

public static class Authentication
{
    public static void AddSecurity(this IServiceCollection services)
    {
        services.AddAuthenticationJwtBearer(s => s.SigningKey = "The secret used to sign the JWT");
        services.AddAuthorization();
    }
}