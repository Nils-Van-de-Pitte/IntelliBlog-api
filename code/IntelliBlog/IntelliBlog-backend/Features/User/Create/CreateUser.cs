using FastEndpoints;

namespace IntelliBlog_backend.Features.User.Create;

public static class CreateUser
{
    public record Request(string Name, string Email, string Password);
    public record Response(Guid Id, string Name, string Email);

    public class Endpoint : Endpoint<Request, Response>
    {
        /// Configures the endpoint route for user registration.
        public override void Configure()
        {
            Post("/api/v1/auth/register");
            AllowAnonymous();
            Tags("Register");
        }


        /// Handles the user registration request and processes the creation of a new user.
        /// <param name="request">The user registration request containing Name, Email, and Password.</param>
        /// <param name="ct">The cancellation token for the asynchronous operation.</param>
        /// <return>A task representing the asynchronous operation that returns a response with the newly created user details.</return>
        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            // Add a user here
            await SendAsync(new Response(
                Id: Guid.NewGuid(),
                Name: request.Name,
                Email: request.Email
            ), 201, ct);
        }
    }
}