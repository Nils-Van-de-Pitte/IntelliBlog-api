using FastEndpoints;
using IntelliBlog_backend.Domain.Interfaces;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Users.Create;

public static class CreateUser
{
    public record Request(string FirstName, string LastName, string Email, string Password);
    public record Response(string Message);

    public class Endpoint(BloggingContext context, IPasswordHasher passwordHasher) : Endpoint<Request, Response>
    {
        
        private readonly BloggingContext _context = context;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;


        /// Configures the endpoint route for user registration.
        public override void Configure()
        {
            Post("/api/v1/auth/register");
            AllowAnonymous();
            Tags("Register");
        }
        
        /// Handles the user registration request and processes the creation of a new user.
        /// <param name="request">The user registration request containing Name, Email, and Password.</param>
        /// <param name="ct">The cancellation token for the asynchronous operation.</param>
        /// <return>A task representing the asynchronous operation that returns a response with the newly created user details.</return>
        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user", ct);

            if (defaultRole == null)
            {
                await SendAsync(new Response("Default role not found!"), 500, ct);
                return;           
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Firstname = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = defaultRole,
                Password = _passwordHasher.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
            
            await SendAsync(new Response("User has been successfully created!"), 201, ct);
        }
    }
}