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
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // auto-dele
        
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.ContactUser)
            .WithMany()
            .HasForeignKey(c => c.ContactId)
            .OnDelete(DeleteBehavior.Restrict); // auto-dele

        modelBuilder.Entity<Contact>()
            .Property(c => c.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Contact>()
            .HasIndex(e => new { e.UserId, e.ContactId })
            .IsUnique();
        modelBuilder.Entity<Contact>()
            .Property(e => e.UserId)
            .IsRequired();
        modelBuilder.Entity<Contact>()
            .Property(e => e.ContactId)
            .IsRequired();
    }
}