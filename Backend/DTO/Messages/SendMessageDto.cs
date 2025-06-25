namespace Chatly.DTO.Messages;

public class SendMessageDto
{
    public string? ContactId { get; set; }
    public string? Content { get; set; }
    public string? ReplyMessageId { get; set; }
    public string? ForwardMessageId { get; set; }
}

public class MessageResponseDto
{
    public string? Id { get; set; }
    public string? ContactId { get; set; }
    public string? Content { get; set; }
    public string? SenderId { get; set; }
    public ForwardMessageResponseDto? ForwardMessage { get; set; }
    public ReplyMessageResponseDto? ReplyMessage { get; set; }
}

public class ForwardMessageResponseDto
{
    public string? Id { get; set; }
    public string? SubContent { get; set; }
    public string? PreviousContactId { get; set; }
    public string? PreviousSenderId { get; set; }
}

public class ReplyMessageResponseDto
{
    public string? Id { get; set; }
    public string? PreviousContent { get; set; }
    public string? PreviousSenderId { get; set; }
}