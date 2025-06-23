using FastEndpoints;
using FluentValidation;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Blogs.Edit;

public class EditBlog
{
    public record BlogReq(string Title, string Description);
    public record BlogRes(string Message);

    /// Represents an endpoint for editing an existing blog.
    /// This endpoint handles HTTP PATCH requests to update a blog's title and description.
    /// It also enforces validation rules for the request payload and applies rate limiting.
    public sealed class Endpoint(BloggingContext context) : Endpoint<BlogReq, BlogRes>
    {
        private readonly BloggingContext _context = context;

        /// Configures the endpoint route for editing a blog.
        public override void Configure()
        {
            Patch("/api/v1/blog/{blogId}");
            AllowAnonymous();
            Tags("Blog");
            Throttle(
                hitLimit: 15,
                durationSeconds: 10
            );
        }

        /// Handles the request to edit an existing blog's details.
        /// This method updates the blog's title and description in the database.
        /// <param name="req">The request object containing the new title and description of the blog.</param>
        /// <param name="ct">The cancellation token to propagate notification of request cancellation.</param>
        /// <return>A task representing the asynchronous operation.</return>
        public override async Task HandleAsync(BlogReq req, CancellationToken ct)
        {
            try
            {
                var blogId = Route<Guid>("blogId");
                var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blogId, ct);
                if (blog == null)
                {
                    AddError("Blog does not exist.");
                    await SendErrorsAsync(404, ct);
                    return;
                }
                
                blog.Title = req.Title;
                blog.Description = req.Description;

                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync(ct);

                await SendAsync(new BlogRes("Blog has been successfully updated!"), 200, ct);
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

    /// Provides validation rules for editing a blog request payload.
    /// This class ensures that the title and description of the blog meet specified constraints
    /// such as non-emptiness and length limits.
    public class Validation : Validator<BlogReq>
    {
        /// Defines the validation rules for the `EditBlog` feature.
        /// Ensures that the `Title` and `Description` fields in the blog update request meet specific requirements,
        /// such as non-emptiness and length constraints.
        public Validation()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(50)
                .WithMessage("Title cannot exceed 20 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}