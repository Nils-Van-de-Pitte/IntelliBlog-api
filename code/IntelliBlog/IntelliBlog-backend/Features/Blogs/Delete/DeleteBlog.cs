using FastEndpoints;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Blogs.Delete;

public class DeleteBlog
{
    public record DeleteBlogRes(string Message);

    public sealed class Endpoint(BloggingContext context) : Ep.NoReq.Res<DeleteBlogRes>
    {
        private readonly BloggingContext _context = context;

        /// Configures the endpoint for the "DeleteBlog" feature.
        public override void Configure()
        {
            Delete("/api/v1/blog/{blogId}");
            AllowAnonymous();
            Tags("Blog");
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        /// Handles the deletion of a blog by its identifier.
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. Returns an appropriate response based on the result of the deletion.</returns>
        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                var blogId = Route<Guid>("blogId");
                var blog = await _context.Blogs.AnyAsync(x => x.Id == blogId, ct);
                if (!blog)
                {
                    AddError("Blog does not exist.");
                    await SendErrorsAsync(404, ct);
                    return;
                }

                await _context.Blogs.Where(b => b.Id == blogId)
                    .ExecuteDeleteAsync(ct);
                
                await SendAsync(new DeleteBlogRes("Blog has been successfully deleted!"), 200, ct);
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
}