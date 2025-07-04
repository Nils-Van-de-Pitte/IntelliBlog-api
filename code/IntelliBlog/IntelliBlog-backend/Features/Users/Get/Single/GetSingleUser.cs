using FastEndpoints;
using IntelliBlog_backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Features.Users.Get.Single;

public class GetSingleUser
{
    public record UserResDto
    {
        public Guid UserId { get; init; }
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public required string RoleName { get; init; }
    }
    
    public record UserRes(string Message, UserResDto User);

    public sealed class Endpoint (BloggingContext context) : Ep.NoReq.Res<UserRes>
    {
        private readonly BloggingContext _context = context;

        /// Configures the endpoint behavior and metadata.
        /// This method is used to specify the HTTP method, route, authentication settings, and tag(s) for the endpoint implementation.
        public override void Configure()
        {
            Get("/api/v1/user/{userId}");
            AllowAnonymous();
            Tags("User");
        }

        /// Handles the execution logic for the endpoint to retrieve a single user.
        /// This method processes the request, fetches the user details based on the provided user ID in the route, and sends the appropriate response.
        /// <param name="ct">A cancellation token to propagate notification that the operation should be canceled.</param>
        /// <return>An asynchronous task representing the operation to send a user response or error message.</return>
        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = Route<Guid>("userId");
            
            var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == userId, ct);
            if (user == null)
            {
                AddError("User does not exist.");
                await SendErrorsAsync(404, ct);
                return;
            }

            var userResponseDto = new UserResDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role.Name,
            };
            await SendAsync(new UserRes("User found!", userResponseDto), 200, ct);
        }
    }
}