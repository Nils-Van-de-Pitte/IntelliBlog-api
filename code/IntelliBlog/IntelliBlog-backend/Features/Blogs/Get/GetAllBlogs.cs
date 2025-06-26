using System.Security.Claims;
using FastEndpoints;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Blogs.Get;

public class GetAllBlogs
{
    public record SingleBlogResDto
    {
        public Guid BlogId { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public required int AmountOfPosts { get; init; }
        public required int AmountOfLikes { get; init; }
    }
    
    public record SingleBlogRes(string Message, List<SingleBlogResDto> Blogs);

    public sealed class Endpoint(BloggingContext context) : Ep.NoReq.Res<SingleBlogRes>
    {
        private readonly BloggingContext _context = context;
        
        public override void Configure()
        {
            Get("api/v1/blogs");
            Roles("user");
            Tags("Blog");
            Description(options =>
                {
                    options.WithSummary("Get all my blogs");
                }
            );
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var blogs = _context.Blogs.Include(x => x.Posts).Where(x => x.UserId == Guid.Parse(userId!));
            if (!blogs.Any())
            {
                AddError("No Blogs found. You can create one by clicking the 'Create Blog' button on the top right corner.");
                await SendErrorsAsync(404, ct);
                return;
            }
            var blogsDto = await blogs
                .Select(blog => new SingleBlogResDto
                {
                    BlogId = blog.Id,
                    Title = blog.Title,
                    Description = blog.Description,
                    AmountOfPosts = blog.Posts.Count,
                    AmountOfLikes = blog.Posts.Sum(p => p.Likes)
                }).ToListAsync(ct);
            
            await SendAsync(new SingleBlogRes("Blogs retrieved successfully!", blogsDto), 200, ct);
        }
    }
}