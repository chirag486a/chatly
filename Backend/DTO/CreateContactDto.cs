using Chatly.Models;

namespace Chatly.DTO;

// ----------------------------
// 👤 5 & 6: Contact Deletion / Blocking
// ----------------------------
public class CreateContactRequestDto
{
    public string ContactUserId { get; set; } = string.Empty;
}

public class CreateContactResponseDto
{
    public string Id { get; set; } = "";
    public string ContactId { get; set; } = "";
    public string UserId { get; set; } = "";
    public ContactStatus? Status = ContactStatus.Pending;
    public DateTime? CreatedAt = DateTime.Now;
    public bool ChatDeleted { get; set; } = false;
    public bool Mutated { get; set; } = false;
    public bool Archived { get; set; } = false;
    public int UnreadCount { get; set; } = 0;
}