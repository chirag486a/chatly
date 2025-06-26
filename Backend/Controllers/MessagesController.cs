using System.Net;
using Chatly.DTO;
using Chatly.DTO.Messages;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = Chatly.Exceptions.ApplicationException;
using Chatly.DTO.Messages;
using Chatly.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chatly.Controllers;

[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    IMessageRepository _repository;
    IHubContext<MessageHub> _hub;

    public MessagesController(IMessageRepository repository, IHubContext<MessageHub> hub)
    {
        _repository = repository;
        _hub = hub;
    }

    [HttpPost("[action]")]
    [Authorize]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null) throw new ApplicationUnauthorizedAccessException("You are not logged in");
            var (newMessage, newReplyMessage, newForwardMessage, contact) = await _repository.CreateAsync(
                contactId: request.ContactId,
                senderId: currUser,
                content: request.Content,
                replyMessageId: request.ReplyMessageId,
                forwardMessageId: request.ForwardMessageId
            );
            var response = new MessageResponseDto
            {
                Id = newMessage.Id,
                ContactId = newMessage.ContactId,
                Content = newMessage.Content
            };
            if (newReplyMessage != null)
            {
                response.ReplyMessage = new ReplyMessageResponseDto
                {
                    Id = newReplyMessage.Id,
                    PreviousContent = newReplyMessage.PreviousContent,
                    PreviousSenderId = newReplyMessage.PreviousSenderId,
                };
            }

            if (newForwardMessage != null)
            {
                response.ForwardMessage = new ForwardMessageResponseDto
                {
                    Id = newForwardMessage.Id,
                    PreviousContactId = newForwardMessage.PreviousContactId,
                    PreviousSenderId = newForwardMessage.PreviousSenderId,
                    SubContent = newForwardMessage.SubContent
                };
            }

            var receiver = contact.UserId == currUser ? contact.ContactId : contact.UserId;
            if (receiver == null) throw new NotFoundException("Receiver does not exist");
            await _hub.Clients.User(receiver)
                .SendAsync("ReceiveMessage", ApiResponse<MessageResponseDto>.SuccessResponse(response));
            Console.WriteLine(receiver);

            return CreatedAtAction(nameof(ReadMessage), ApiResponse<MessageResponseDto>.SuccessResponse(response,
                "Message sent", null,
                StatusCodes.Status201Created));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, ex.StatusCode, ex.ErrorCode, ex.Details,
                ex.Errors));
        }
        catch (ApplicationUnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse(ex.Message, ex.StatusCode, ex.ErrorCode, ex.Details,
                ex.Errors));
        }
        catch (ApplicationArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message, ex.StatusCode, ex.ErrorCode, ex.Details,
                ex.Errors));
        }
        catch (ConflictException ex)
        {
            return Conflict(ApiResponse<object>.ErrorResponse(ex.Message, ex.StatusCode, ex.ErrorCode, ex.Details,
                ex.Errors));
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadMessage([FromQuery] string messageId)
    {
        throw new NotImplementedException();
    }
}