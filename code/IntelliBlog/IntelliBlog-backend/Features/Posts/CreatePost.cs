using FastEndpoints;
using FluentValidation;
using IntelliBlog_backend.Infrastructure.Database;

namespace IntelliBlog_backend.Features.Posts;

public class CreatePost
{
    public record PostReq(string Title, string Content);
    public record PostRes(string Message);

    public sealed class Endpoint(BloggingContext context) : Endpoint<PostReq, PostRes> 
    {
        private readonly BloggingContext _context = context;

        public override void Configure()
        {
            Post("/api/v1/post");
            AllowAnonymous();
            Description(options =>
            {
                options.WithSummary("Create a new post");
            });
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        public override async Task HandleAsync(PostReq request, CancellationToken ct)
        {
            try
            {
                var postExists = _context.Posts.Any(p => p.Title == request.Title);
                
                if (postExists)
                {
                    AddError("Post with this title already exists.");
                    await SendErrorsAsync(409, ct);
                    return;
                }
                
                // Hardcode for testing purposes
                var blogId = Guid.Parse("29b038ca-2981-4d61-b8c8-19e294bc7b6f"); //TODO when you find the JWT, insert the ID here instead

                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    Likes = 0,
                    BlogId = blogId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Posts.AddAsync(post, ct);
                await _context.SaveChangesAsync(ct);

                await SendAsync(new PostRes("Post has been successfully created!"), 201, ct);
            }
            catch (Exception e)
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    AddError(e.Message);
                }
                await SendErrorsAsync(500, ct);
            }
            
        }
    }

    public sealed class Validation : Validator<PostReq>
    {
        public Validation()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required")
                .MaximumLength(1000)
                .WithMessage("Content cannot exceed 1000 characters");
        }
    }
}