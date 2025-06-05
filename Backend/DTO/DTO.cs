namespace Chatly.DTO;



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