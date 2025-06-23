using IntelliBlog_backend.Domain.Extensions;
using IntelliBlog_backend.Domain.Extensions.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Services.RegisterServices();

var app = builder.Build();

app.UseSecurity();

app.UseRoutes();

app.AddSwagger();

app.UseHttpsRedirection();

app.Run();