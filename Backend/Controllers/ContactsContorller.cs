using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationException = Chatly.Exceptions.ApplicationException;

namespace Chatly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _contactRepository;

    public ContactsController(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }


    [HttpPost]
    public async Task<IActionResult> SendRequest(CreateContactRequestDto request)
    {
        try
        {
            var currUserId = User.GetUserId();
            var currUserName = User.GetUserName();
            if (currUserId == null || currUserName == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null").AddError(
                    "UserId", "User id is null");
            }

            var existingContact = await _contactRepository.GetAsync(userId: currUserId,
                contactId: request.ContactUserId, contactUserName: request.ContactUserName);
            if (existingContact == null)
            {
                existingContact =
                    await _contactRepository.Create(
                        userId: currUserId,
                        currUserName: User.GetUserName(),
                        contactUserId: request.ContactUserId,
                        contacctUsername: request.ContactUserName
                    );
            }

            if (existingContact.Status == ContactStatus.None)
            {
                existingContact =
                    await _contactRepository.UpdateAsync(contactId: existingContact.Id,
                        contactStatus: ContactStatus.Pending);
            }
            else
                throw new ConflictException("Cant send request", $"The contact status is {existingContact.Status}")
                    .AddError("ContactStatus", $"The contact status is {existingContact.Status}");


            return CreatedAtAction(nameof(GetContact), new { contactId = existingContact.Id },
                ApiResponse<CreateContactResponseDto>.SuccessResponse(new CreateContactResponseDto
                    {
                        Id = existingContact.Id,
                        RequestStatus = existingContact.Status.ToString() ?? "ERR",
                    }, "Added contact", null,
                    StatusCodes.Status201Created));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ApplicationArgumentException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ConflictException e)
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors)
            );
        }
        catch (ApplicationException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (Exception e)
        {
            return this.InternalServerError(ApiResponse<object>.ErrorResponse("Something went wrong"));
        }
    }

    [HttpGet("{contactId}")]
    [Authorize]
    public async Task<IActionResult> GetContact([FromRoute] string contactId)
    {
        var c = await _contactRepository.GetAsync(contactId: contactId);
        return Ok(ApiResponse<Contact?>.SuccessResponse(c, "Contact found", null, statusCode: StatusCodes.Status200OK));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserContacts([FromQuery] GetUserContactsRequestDto request)
    {
        try
        {
            var curUser = User.GetUserId();
            if (curUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var (contactList, count) = await _contactRepository.GetAllAsync(curUser, request.Page, request.PageSize,
                excludeBlocked: false, excludeNone: false, onlyBlocked: false, onlyNone: false);

            return Ok(ApiResponse<List<Contact>>.SuccessResponse(
                data: contactList,
                message: "Contacts",
                totalCount: count,
                statusCode: StatusCodes.Status200OK));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    [HttpPatch("request/{contactId}")]
    public async Task<IActionResult> ModifyRequest([FromRoute] string contactId,
        [FromBody] AcceptRequestRequestDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var contact = await _contactRepository.GetAsync(contactId);
            if (contact.Status != ContactStatus.Pending)
            {
                throw new ConflictException("Unable to accept contact", "Request is not pending");
            }

            contact = await _contactRepository.UpdateAsync(contactId: contact.ContactId,
                contactStatus: request.IsAccepted ? ContactStatus.Accepted : ContactStatus.None);

            return Accepted(ApiResponse<AcceptRequestResponseDto>.SuccessResponse(new AcceptRequestResponseDto

                {
                    ContactId = contact.Id,
                },
                contact.Status == ContactStatus.Accepted
                    ? "Request accepted successfully"
                    : "Request rejected successfully", null,
                StatusCodes.Status200OK));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ConflictException e)
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (InternalServerException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    [HttpPatch("block")]
    [Authorize]
    public async Task<IActionResult> Block([FromBody] BlockRequestDto request)
    {
        try
        {
            var currUserId = User.GetUserId();
            var currUserName = User.GetUserName();
            if (currUserId == null || currUserName == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var contact = await _contactRepository.UpdateAsync(request.ContactId, contactStatus: ContactStatus.Blocked);

            return Accepted(ApiResponse<BlockResponseDto>.SuccessResponse(new BlockResponseDto
                {
                    ContactId = contact.Id,
                }, request.IsBlocked ? "Blocked successfully" : "Unblocked successfully", null,
                StatusCodes.Status200OK));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ConflictException e)
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (InternalServerException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }
}