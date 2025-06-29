namespace Chatly.DTO.Messages;

/// <summary>
/// Data Transfer Object used to request permanent deletion of a message.
/// </summary>
public class DeleteMessageDto
{
    /// <summary>
    /// The unique identifier of the message to be deleted.
    /// </summary>
    public string? MessageId { get; set; }
}