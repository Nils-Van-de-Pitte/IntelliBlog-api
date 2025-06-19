namespace IntelliBlog_backend.Domain.Extensions;

public static class Swagger
{
    public static void AddSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}