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
            Console.WriteLine(User.GetUserName());
            if (currUser == null) throw new ApplicationUnauthorizedAccessException("You are not logged in");
            var newMessage = await _repository.CreateAsync(
                contactId: request.ContactId,
                senderId: currUser,
                content: request.Content,
                replyMessageId: request.ReplyMessageId,
                forwardMessageId: request.ForwardMessageId
            );
            var responseDto = newMessage.ToMessageResponseDto();

            var receiver = newMessage.Contact?.UserId == currUser
                ? newMessage.Contact.ContactId
                : newMessage.Contact?.UserId;
            if (receiver == null) throw new NotFoundException("Receiver does not exist");
            await _hub.Clients.User(receiver)
                .SendAsync("ReceiveMessage", ApiResponse<MessageResponseDto>.SuccessResponse(responseDto));
            Console.WriteLine(receiver);

            return CreatedAtAction(nameof(ReadMessages), ApiResponse<MessageResponseDto>.SuccessResponse(responseDto,
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
    [Authorize]
    public async Task<IActionResult> ReadMessages([FromQuery] ReadMessageDto request)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null) throw new ApplicationUnauthorizedAccessException("You are not logged in");


            var (messages, count) =
                await _repository.GetAllAsync(request.ContactId, userId, request.Page ?? 1, request.PageSize ?? 10);


            return Ok(ApiResponse<List<MessageResponseDto>>.SuccessResponse(messages.ToListMessageResponseDto(),
                "Messages", count, StatusCodes.Status200OK));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
            throw;
        }
        catch (NotFoundException e)
        {
            return BadRequest(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ConflictException e)
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> EditMessage([FromBody] EditMessageDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("You are not logged in");
            }

            var newMessage = await _repository.EditMessageAsync(request.MessageId, currUser, request.NewContent);
            //  Ensure that Contact is included in model!
            var reciever = newMessage.Contact?.UserId == currUser
                ? newMessage.Contact.ContactId
                : newMessage.Contact?.UserId;

            await _hub.Clients.User(reciever ?? currUser).SendAsync("MessageEdited",
                ApiResponse<MessageResponseDto>.SuccessResponse(
                    newMessage.ToMessageResponseDto(),
                    "Message saved", null, StatusCodes.Status202Accepted
                )
            );
            return Accepted(
                ApiResponse<MessageResponseDto>.SuccessResponse(
                    newMessage.ToMessageResponseDto(),
                    "Message saved", null, StatusCodes.Status202Accepted
                )
            );
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    /// <summary>
    /// Permanently deletes a message from the database.
    /// </summary>
    /// <param name="request">
    /// A <see cref="DeleteMessageDto"/> object containing the necessary information to identify and authorize the deletion request.
    /// </param>
    /// <returns>
    /// Returns an appropriate <see cref="IActionResult"/> based on the outcome:
    /// <list type="bullet">
    ///   <item><description><see cref="UnauthorizedResult"/> if the user is not authorized to delete the message.</description></item>
    ///   <item><description><see cref="NotFoundResult"/> if the message does not exist.</description></item>
    ///   <item><description><see cref="NoContentResult"/> if the message was successfully deleted.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Exceptions or error states (Unauthorized, NotFound) are thrown from the repository layer and handled accordingly.
    /// </remarks>
    [HttpDelete("[action]")]
    [Authorize]
    public async Task<IActionResult> DeleteMessage([FromBody] DeleteMessageDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
                throw new ApplicationUnauthorizedAccessException("You are not logged in");
            await _repository.DeleteMessageAsync(request.MessageId, currUser);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }
}