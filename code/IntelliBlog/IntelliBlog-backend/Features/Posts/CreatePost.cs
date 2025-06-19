using FastEndpoints;
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
    }
}