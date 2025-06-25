namespace Chatly.Models;

public class ReplyMessage
{
    public string? Id { get; set; }
    public string? MessageId { get; set; }
    public string? PreviousSenderId { get; set; }
    public string? PreviousContent { get; set; }

    public Message? Message { get; set; }
    public User? PreviousSender { get; set; }
}