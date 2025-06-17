using IntelliBlog_backend.Domain.Interfaces;

namespace IntelliBlog_backend.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    /// Hashes a password using a secure hashing algorithm.
    /// <param name="password">The plain text password to be hashed.</param>
    /// <return>The hashed password as a string.</return>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// Verifies if the provided password matches the hashed password.
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <return>True if the password matches the hashed password; otherwise, false.</return>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}