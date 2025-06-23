using FastEndpoints;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Posts.Likes.Add;

public class AddLike
{
    public record LikeRes(string Message);
    
    public sealed class Endpoint(BloggingContext context) : Ep.NoReq.Res<LikeRes>
    {
        private readonly BloggingContext _context = context;
        
        /// Configures the endpoint for handling HTTP Patch to adding a like to a specific post.
        public override void Configure()
        {
            Patch("/api/v1/post/{postId}/like");
            AllowAnonymous();
            Description(options =>
            {
                options.WithSummary("Add a like to a post");


            });
            Throttle(
                hitLimit: 5,
                durationSeconds: 10
            );
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var postId = Route<Guid>("postId");
            var postExists = _context.Posts.AnyAsync(p => p.Id == postId, ct);
            if (!postExists.Result)
            {
                AddError("No post with this ID exists.");
                await SendErrorsAsync(404, ct);
                return;
            }
            await _context.Posts.Where(p => p.Id == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(post => post.Likes, post => post.Likes + 1), ct);
            await SendAsync(new LikeRes("Like added!"), 200, ct);
        }
    }
}