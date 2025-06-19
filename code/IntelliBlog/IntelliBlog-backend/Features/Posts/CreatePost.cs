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
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    Likes = 0,
                    BlogId = Guid.NewGuid(),
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