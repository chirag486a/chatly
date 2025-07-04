using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Models;

public enum ContactStatus
{
    None,
    Pending, // pending
    Accepted, // accepted
    Blocked,  // blocked
    Deleted
}

[PrimaryKey(nameof(Id))]
public class Contact
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? ContactId { get; set; }
    public ContactStatus? Status { get; set; } = ContactStatus.Pending;
    public DateTime? CreatedAt { get; set; }
    public bool ChatDeleted { get; set; }
    public bool Mutated { get; set; }
    public bool Archived { get; set; }
    public int UnreadCount { get; set; }

    [ForeignKey(nameof(UserId))] public User? User { get; set; }

    [ForeignKey(nameof(ContactId))] public User? ContactUser { get; set; }
}