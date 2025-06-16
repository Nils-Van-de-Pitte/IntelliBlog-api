using Microsoft.EntityFrameworkCore;

namespace IntelliBlog_backend.Infrastructure.Database;

public class BloggingContext: DbContext
{
    public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles {get; set;}
    public DbSet<User> Users {get; set;}
    public DbSet<Blog> Blogs {get; set;}
    public DbSet<Post> Posts {get; set;}
}