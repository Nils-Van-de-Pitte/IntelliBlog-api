using System.Security.Claims;
using FastEndpoints;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Posts.Get;

public class AllPosts
{
    public record AllPostResDto
    {
        public Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Content { get; init; }
    }
    public record AllPostRes(ICollection<AllPostResDto> Results);

    public sealed class Endpoint(BloggingContext context) : Ep.NoReq.Res<AllPostRes>
    {
        private readonly BloggingContext _context = context;
        public override void Configure()
        {
            Get("/api/v1/posts");
            Roles("user");
            Description(options =>
            {
                options.WithSummary("Get all my posts");
            });
            Tags("Posts");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blogs = _context.Posts.Where(x => x.UserId == Guid.Parse(userId!));

            var postResDtos = await blogs.Select(x => new AllPostResDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content
            }).ToListAsync(ct);
            
            await SendAsync(new AllPostRes(postResDtos), 200, ct);
        }
    }
}