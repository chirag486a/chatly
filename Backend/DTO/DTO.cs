namespace Chatly.DTO;


// ----------------------------
// 🔍 4: User Search DTOs
// ----------------------------
public class UserSearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}


// ----------------------------
// 👤 5 & 6: Contact Deletion / Blocking
// ----------------------------
public class ContactActionDto
{
    public Guid ContactUserId { get; set; }
}

public class ContactActionResponseDto
{
    public string Status { get; set; } = "Success";
}

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------
public class SendRequestDto
{
    public Guid ContactUserId { get; set; }
}

public class SendRequestResponseDto
{
    public string RequestStatus { get; set; } = "Pending"; // or Accepted/Rejected
}

// ----------------------------
// ⚙️ 8: Update Profile Info
// ----------------------------
public class UserUpdateDto
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Theme { get; set; }
}

public class UserUpdateResponseDto
{
    public string Status { get; set; } = "Updated";
}

// ----------------------------
// ❌ 9: Delete User Account
// ----------------------------
public class DeleteUserDto
{
    public string PasswordConfirmation { get; set; } = string.Empty;
}

public class DeleteUserResponseDto
{
    public string Status { get; set; } = "Deleted";
}

// ----------------------------
// 💬 10: Messaging DTOs (with parent_message_id)
// ----------------------------
public class SendMessageDto
{
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentMessageId { get; set; }
}

public class MessageResponseDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public bool Read { get; set; }
    public bool Delivered { get; set; }
    public Guid? ParentMessageId { get; set; }
}

// ----------------------------
// 🔁 Forwarded Message DTOs
// ----------------------------
public class ForwardMessageDto
{
    public Guid MessageId { get; set; }
    public Guid NewReceiverId { get; set; }
    public string? AdditionalMessage { get; set; }
}

public class ForwardedMessageResponseDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid NewReceiverId { get; set; }
    public string? AdditionalMessage { get; set; }
    public DateTime ForwardedAt { get; set; }
}

// ----------------------------
// ✅ Real-time Delivery Acknowledgement DTOs
// ----------------------------
public class MessageDeliveryUpdateDto
{
    public Guid MessageId { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsRead { get; set; }
}

public class DeliveryStatusResponseDto
{
    public string Status { get; set; } = "Updated";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}