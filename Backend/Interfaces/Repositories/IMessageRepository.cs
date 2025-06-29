using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IMessageRepository
{
    public Task<Message> CreateAsync(
        string? contactId,
        string? senderId,
        string? content,
        string? replyMessageId = null,
        string? forwardMessageId = null
    );

    public Task<(List<Message>, int)> GetAllAsync(
        string? contactId,
        string? userId,
        int page = 1,
        int pageSize = 10
    );

    public Task<Message> EditMessageAsync(
        string messageId,
        string? senderId,
        string? content
    );
}