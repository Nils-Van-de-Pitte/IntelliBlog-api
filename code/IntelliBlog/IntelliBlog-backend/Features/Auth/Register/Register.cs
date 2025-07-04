using FastEndpoints;
using FluentValidation;
using IntelliBlog_backend.Domain.Interfaces;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Auth.Register;

public static class Register
{
    public record RegisterReq(string FirstName, string LastName, string Email, string Password);
    public record RegisterRes(string Message);

    public sealed class Endpoint(BloggingContext context, IPasswordHasher passwordHasher) : Endpoint<RegisterReq, RegisterRes>
    {
        
        private readonly BloggingContext _context = context;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        
        /// Configures the endpoint route for user registration.
        public override void Configure()
        {
            Post("/api/v1/auth/register");
            AllowAnonymous();
            Tags("Auth");
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }
        
        /// Handles the user registration registerReq and processes the creation of a new user.
        /// <param name="registerReq">The user registration registerReq containing Name, Email, and Password.</param>
        /// <param name="ct">The cancellation token for the asynchronous operation.</param>
        /// <return>A task representing the asynchronous operation that returns a response with the newly created user details.</return>
        public override async Task HandleAsync(RegisterReq registerReq, CancellationToken ct)
        {
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user", ct);

            if (defaultRole == null)
            {
                AddError("Default role is not found");
                await SendErrorsAsync(404, ct);
                return;
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Firstname = registerReq.FirstName,
                LastName = registerReq.LastName,
                Email = registerReq.Email,
                Role = defaultRole,
                Password = _passwordHasher.HashPassword(registerReq.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
            
            await SendAsync(new RegisterRes("User has been successfully created!"), 201, ct);
        }
    }
    
    public class Validation : Validator<RegisterReq>
    {
        public Validation()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(20)
                .WithMessage("First name cannot exceed 20 characters.");
            RuleFor(x => x.LastName)
                .MaximumLength(20)
                .WithMessage("Last name cannot exceed 20 characters.");
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email is not valid.");
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");
            
        }
    }
}