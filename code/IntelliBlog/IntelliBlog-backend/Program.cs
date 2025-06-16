using IntelliBlog_backend.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.UseDependencies(builder.Configuration);

var app = builder.Build();

app.UseRoutes();

app.UseScalar();

app.UseHttpsRedirection();

app.Run();