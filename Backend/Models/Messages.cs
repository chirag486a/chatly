namespace Chatly.Models;

public class Message
{
    public string? Id { get; set; }
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    public string? Content { get; set; }
    public string? SeenAt { get; set; }
    public bool Read { get; set; }
    public string? ParentMessageId { get; set; }
    public bool? IsForwarded { get; set; }
    
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public Message? ParentMessage { get; set; }
}