using Chatly.Models;

namespace Chatly.DTO.Contacts;

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------

public class CreateContactRequestDto
{
    public string? ContactUserId { get; set; }
    public string? ContactUserName { get; set; }
}

public class ContactResponseDto
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? ContactId { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool ChatDeleted { get; set; }
    public bool Mutated { get; set; }
    public bool Archived { get; set; }
    public int UnreadCount { get; set; }
}