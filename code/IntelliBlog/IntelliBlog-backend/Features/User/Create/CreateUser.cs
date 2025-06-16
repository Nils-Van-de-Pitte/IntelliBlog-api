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

        /// Handles the user registration process by processing the incoming request and returning a response.
        /// <param name="request">
        /// The incoming request containing user details such as name, email, and password for registration.
        /// </param>
        /// <param name="ct"></param>
        /// <returns>
        /// A result object containing the user details including ID, name, and email if registration is successful.
        /// </returns>
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