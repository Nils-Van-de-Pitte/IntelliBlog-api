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
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
// DEPRECATED
/*public sealed class Blog
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public required string Description { get; set; }
    public required Guid UserId { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; } 
}*/
public sealed class Post
{
    public Guid Id { get; init; }
    [MaxLength(20)]
    public required string Title { get; set; }
    [MaxLength(20)]
    public required string Content { get; set; }
    public required Guid UserId { get; set; }
    [MaxLength(20)]
    public required int Likes { get; set; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public sealed class Comment
{
    public Guid Id { get; init; }
    [MaxLength(1000)]
    public required string Content { get; set; }
    public required Guid PostId { get; set; }
    public required Post Post { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime CreatedAt { get; init; }
}

public sealed class Like
{
    public Guid Id { get; init; }
    public required Guid PostId { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime CreatedAt { get; init; }
}
