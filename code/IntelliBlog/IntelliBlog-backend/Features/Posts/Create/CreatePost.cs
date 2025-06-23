using FastEndpoints;
using FluentValidation;
using IntelliBlog_backend.Infrastructure.Database;

namespace IntelliBlog_backend.Features.Posts.Create;

public class CreatePost
{
    public record PostReq(string Title, string Content);
    public record PostRes(string Message);

    /// <summary>
    /// Represents an HTTP endpoint for creating new blog posts in the system.
    /// </summary>
    /// <remarks>
    /// This class defines the behavior and configuration of the endpoint, including routing, access control, request validation,
    /// and rate limiting. It processes requests from clients to create posts, interacts with the database for persistence,
    /// and provides appropriate success or error responses.
    /// </remarks>
    public sealed class Endpoint(BloggingContext context) : Endpoint<PostReq, PostRes> 
    {
        private readonly BloggingContext _context = context;

        /// <summary>
        /// Configures the endpoint for handling HTTP POST requests to create new blog posts, including route definition, access settings, description, and rate limiting.
        /// </summary>
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

        /// <summary>
        /// Handles the asynchronous creation of a new blog post, including validation, database persistence, and response generation.
        /// </summary>
        /// <param name="request">The request object containing the title and content of the post.</param>
        /// <param name="ct">The cancellation token used to propagate notifications of request cancellation.</param>
        /// <returns>A task that represents the asynchronous operation. On success, a response with a success message is sent. On failure, appropriate error responses are sent.</returns>
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

    /// <summary>
    /// Provides validation rules for the CreatePost feature, ensuring the integrity and consistency of user input
    /// for creating a new post.
    /// </summary>
    /// <remarks>
    /// The validation rules include checks for required fields and constraints on the maximum length of the content.
    /// </remarks>
    public sealed class Validation : Validator<PostReq>
    {
        /// <summary>
        /// Represents the validation logic for the CreatePost feature, ensuring that the required fields for creating a new blog post are properly validated.
        /// </summary>
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