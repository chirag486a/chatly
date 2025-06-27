namespace Chatly.DTO.Messages;

public class ReadMessageDto
{
    public string? ContactId { get; set; }

    public int? PageSize { get; set; }

    public int? Page { get; set; }
}