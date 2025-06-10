using Chatly.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ForwardedMessage> ForwardedMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .Property(c => c.Status)
            .HasConversion<string>();
    }
}