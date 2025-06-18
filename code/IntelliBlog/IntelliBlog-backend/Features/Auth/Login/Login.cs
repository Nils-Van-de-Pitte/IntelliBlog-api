using FastEndpoints;
using FastEndpoints.Security;
using IntelliBlog_backend.Domain.Interfaces;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Auth.Login;

public static class Login
{
    public record LoginReq(string Email, string Password);
    public record LoginRes(string Message, string Token);

    public sealed class Endpoint(IPasswordHasher passwordHasher, BloggingContext context, IConfiguration configuration) : Endpoint<LoginReq, LoginRes>
    {
        private readonly BloggingContext _context = context;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IConfiguration _configuration = configuration;
        
        /// Configures the endpoint route for user login.
        public override void Configure()
        {
            Post("/api/v1/auth/login");
            AllowAnonymous();
            Tags("Auth");
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        /// Handles the login request and processes user authentication.
        /// <param name="req">The request object containing login credentials (email and password).</param>
        /// <param name="ct">The cancellation token for the asynchronous operation.</param>
        /// <return>A task representing the asynchronous operation, yielding a response containing a message and a token upon success.</return>
        public override async Task HandleAsync(LoginReq req, CancellationToken ct)
        {
            try
            {
                var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == req.Email);
                if (user == null)
                {
                    AddError("Invalid email or password.");
                    await SendErrorsAsync(409, ct);
                    return;
                }
                var passwordMatches = _passwordHasher.VerifyPassword(req.Password, user.Password);
                if (!passwordMatches)
                {
                    AddError("Invalid email or password.");
                    await SendErrorsAsync(409, ct);
                    return;
                }
                var token = GenerateJwt(user);
                await SendAsync(new LoginRes("User has been successfully logged in!", token), 200, ct);
            }
            catch (Exception e)
            {
                AddError("An error occurred while processing your request.");
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    AddError(e.Message);
                }
                await SendErrorsAsync(500, ct);
            }
        }

        /// Generates a JWT (JSON Web Token) for the specified user, containing claims such as email, roles,
        /// user ID, and an expiration time.
        /// <param name="user">The user for whom the JWT is being generated. The user's role and email
        /// are included as claims in the token.</param>
        /// <returns>A string representing the generated JWT, signed and encoded, with the user's claims and expiration time.</returns>
        private string GenerateJwt(User user)
        {
            var token = JwtBearer.CreateToken(options =>
            {
                options.SigningKey = GetJwtSigningKey();
                options.ExpireAt = DateTime.UtcNow.AddDays(1);
                options.User.Roles.Add(user.Role.Name);
                options.User.Claims.Add(("Email", user.Email));
                options.User["UserId"] = user.Id.ToString();
            });
            return token;
        }

        /// Retrieves the signing key used for generating JWT tokens.
        /// <return>The signing key value as a string.</return>
        private string GetJwtSigningKey()
        {
            return (_configuration["JWT_SECRET"] ?? Environment.GetEnvironmentVariable("JWT_SECRET")) 
                   ?? throw new InvalidOperationException("JWT_SECRET not configured");
        }
    }
}