using FastEndpoints;
using IntelliBlog_backend.Domain.Interfaces;
using IntelliBlog_backend.Infrastructure.Database;

namespace IntelliBlog_backend.Features.Auth.Login;

public static class Login
{
    public record LoginReq(string Email, string Password);
    public record LoginRes(string Message, string? Token);

    public sealed class Endpoint(IPasswordHasher passwordHasher, BloggingContext context) : Endpoint<LoginReq, LoginRes>
    {
        private readonly BloggingContext _context = context;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        
        /// Configures the endpoint route for user login.
        public override void Configure()
        {
            Post("/api/v1/auth/login");
            AllowAnonymous();
            Tags("Auth");
        }

        /// Handles the login request and processes user authentication.
        /// <param name="req">The request object containing login credentials (email and password).</param>
        /// <param name="ct">The cancellation token for the asynchronous operation.</param>
        /// <return>A task representing the asynchronous operation, yielding a response containing a message and a token upon success.</return>
        public override async Task HandleAsync(LoginReq req, CancellationToken ct)
        {
            var existingEmail = _context.Users.FirstOrDefault(u => u.Email == req.Email);
            if (existingEmail == null) {await SendErrorsAsync(409, ct); return;}
            var user = _context.Users.FirstOrDefault(u => u.Email == req.Email);
            var passwordMatches = _passwordHasher.VerifyPassword(req.Password, user?.Password!);
            if (!passwordMatches) {await SendErrorsAsync(409, ct); return; }
            await SendAsync(new LoginRes("User has been successfully logged in!", "token"), 200, ct);
        }
    }
}