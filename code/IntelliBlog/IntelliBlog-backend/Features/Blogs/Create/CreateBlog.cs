using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using IntelliBlog_backend.Infrastructure.Database;

namespace IntelliBlog_backend.Features.Blogs.Create;

public class CreateBlog
{
    public record BlogReq(string Title, string Description);
    public record BlogRes(string Message);
    
    public sealed class Endpoint(BloggingContext context) : Endpoint<BlogReq, BlogRes>
    {
        private readonly BloggingContext _context = context;

        /// Configures the endpoint route for the creation of a new blog.
        public override void Configure()
        {
            Post("/api/v1/blog");
            Roles("user");
            Description(options =>
            {
                options.WithSummary("Create a new blog");
            });
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        /// Handles the creation of a new blog by processing the incoming request and saving the blog details in the database.
        /// Validates the request to ensure a blog with the same title does not already exist.
        /// <param name="request">The request containing the title and description of the blog to be created.</param>
        /// <param name="ct">A token to monitor for cancellation requests.</param>
        /// <return>A task representing the asynchronous operation. Returns a response indicating the success or failure of the blog creation.</return>
        public override async Task HandleAsync(BlogReq request, CancellationToken ct)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                Console.WriteLine($"Role: {userRole}");
                var hasBlog = _context.Blogs.Any(b => b.Title == request.Title);
                if (hasBlog)
                {
                    AddError(b => b.Title, "Blogs with this title already exists.");
                    await SendErrorsAsync(409, ct);
                }
                
                var blog = new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync(ct);
                
                await SendAsync(new BlogRes("Blog has been successfully created!"), 201, ct);

            } catch (Exception e)
            { 
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    AddError(e.Message);
                }
                await SendErrorsAsync(500, ct);
            }
        }
    }
    
    public class Validator : Validator<BlogReq>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(20)
                .WithMessage("Title cannot exceed 20 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}