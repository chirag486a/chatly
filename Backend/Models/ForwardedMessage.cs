namespace Chatly.Models;


public class ForwardedMessage
{
    public string? Id { get; set; }
    public string? OriginalMessageId { get; set; }
    public string? NewMessageId { get; set; }
    public string? ForwarderId { get; set; }
    public string? NewReceiverId { get; set; }
    public string? AdditionalMessage { get; set; }
    public DateTime? ForwardedAt { get; set; }
    
    
    public Message? OriginalMessage { get; set; } 
    public Message? NewMessage { get; set; } 
    
    public User? Forwarder { get; set; }
    public User? NewReceiver { get; set; }
    
}