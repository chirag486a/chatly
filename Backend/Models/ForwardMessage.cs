namespace Chatly.Models;

public class ForwardMessage
{
    public string? Id { get; set; }
    public string? MessageId { get; set; }
    public string? PreviousContent { get; set; }
    public string? PreviousContactId { get; set; }
    public string? PreviousSenderId { get; set; }

    public Message? Message { get; set; }
    public Contact? PreviousContact { get; set; }
    public User? PreviousSender { get; set; }
}