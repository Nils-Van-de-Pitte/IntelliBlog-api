using IntelliBlog_backend.Domain.Extensions;
using IntelliBlog_backend.Domain.Extensions.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Services.RegisterServices();

var app = builder.Build();

app.UseRoutes();

app.UseScalar();

app.UseSecurity();

app.UseHttpsRedirection();

app.Run();