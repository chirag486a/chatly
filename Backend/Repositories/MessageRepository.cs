using Chatly.Data;
using Chatly.Exceptions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Repositories;

public class MessageRepository : IMessageRepository
{
    ApplicationDbContext _dbContext;

    public MessageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(Message, ReplyMessage?, ForwardMessage?, Contact)> CreateAsync(string? contactId,
        string? senderId,
        string? content,
        string? replyMessageId = null,
        string? forwardMessageId = null)
    {
        if (string.IsNullOrEmpty(contactId))
            throw new ApplicationArgumentException("ContactId cannot be null or empty.", nameof(contactId));

        if (string.IsNullOrEmpty(senderId))
            throw new ApplicationArgumentException("SenderId cannot be null or empty.", nameof(senderId));

        if (content == null)
            throw new ApplicationArgumentException("Content cannot be null", nameof(content));

        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == contactId);
        if (contact == null) throw new NotFoundException("Contact not found");

        if (contact.Status != ContactStatus.Accepted)
        {
            throw new ConflictException("Contact is not accepted");
        }

        var sender = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == senderId);
        if (sender == null) throw new NotFoundException("Sender not found");

        if (!(contact.ContactId == senderId || contact.UserId == senderId))
        {
            throw new ApplicationUnauthorizedAccessException("You are not authorized to access this contact");
        }

        var replyToMessage =
            replyMessageId != null ? _dbContext.Messages.FirstOrDefault(x => x.Id == replyMessageId) : null;

        if (
            replyToMessage != null &&
            replyToMessage.ContactId != contact.Id
        )
        {
            throw new ConflictException("Cannot reply message to a different contact");
        }

        var messageToBeForwarded =
            await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == forwardMessageId);

        if (messageToBeForwarded != null &&
            messageToBeForwarded.ContactId == contact.Id
           )
        {
            throw new ConflictException("Cannot forward message to a same contact");
        }

        if (messageToBeForwarded != null)
        {
            var previousContact =
                await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == messageToBeForwarded.ContactId);
            if (previousContact == null) throw new NotFoundException("Contact not found");
            _ = previousContact.ContactId == senderId || previousContact.ContactId == contactId
                ? true
                : throw new ApplicationUnauthorizedAccessException("Cannot forward message from unauthorized contact");
        }

        Message newMessage = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ContactId = contact.Id,
            SenderId = sender.Id,
            Content = content
        };
        ReplyMessage? newReplyMessage = null;
        ForwardMessage? newforwardMessage = null;


        if (replyToMessage != null)
        {
            newReplyMessage = new ReplyMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = newMessage.Id,
                PreviousContent = replyToMessage.Content,
                PreviousSenderId = replyToMessage.SenderId
            };
            newMessage.IsReply = true;
            await _dbContext.ReplyMessages.AddAsync(newReplyMessage);
        }

        if (messageToBeForwarded != null)
        {
            newforwardMessage = new ForwardMessage
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = newMessage.Id,
                SubContent = content,
                PreviousContactId = messageToBeForwarded.ContactId,
                PreviousSenderId = messageToBeForwarded.SenderId
            };
            newMessage.Content = messageToBeForwarded.Content;
            newMessage.ContactId = contactId;
            newMessage.IsForwarded = true;
            await _dbContext.ForwardMessages.AddAsync(newforwardMessage);
        }


        await _dbContext.Messages.AddAsync(newMessage);
        await _dbContext.SaveChangesAsync();
        return (newMessage, newReplyMessage, newforwardMessage, contact);
    }
}