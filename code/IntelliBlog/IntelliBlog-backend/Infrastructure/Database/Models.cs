using System.ComponentModel.DataAnnotations;

namespace IntelliBlog_backend.Infrastructure.Database;

public sealed class Role
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
public sealed class User
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Firstname { get; init; }
    [MaxLength(20)]
    public required string LastName { get; init; }
    [MaxLength(20), EmailAddress]
    public required string Email { get; init; }
    [MinLength(20), MaxLength(500)]
    public required string Password { get; set; }
    public required Role Role { get; set; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public string FullName => $"{Firstname} {LastName}";
    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
public sealed class Blog
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public required string Description { get; set; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
public sealed class Post
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Title { get; set; }
    [MaxLength(20)]
    public required string Content { get; set; }
    [MaxLength(20)]
    public required int Likes { get; set; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
