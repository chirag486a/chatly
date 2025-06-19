using System.Net;
using System.Runtime.CompilerServices;
using Chatly.Data;
using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> Create(CreateContactRequestDto request)
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


            var newContact = await _contactRepository.Create(request, currUserId, currUserName);
            return CreatedAtAction("", new { userId = newContact.Id }, // This is the routeValues object
                ApiResponse<CreateContactResponseDto>.SuccessResponse(newContact, "Added contact", null,
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetContacts([FromRoute] GetUserContactsRequestDto request)
    {
        try
        {
            var curUser = User.GetUserId();
            if (curUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var response = await _contactRepository.GetAllUserContactsAsync(request, curUser);

            return Ok(ApiResponse<List<Contact>>.SuccessResponse(
                data: response.Contacts,
                message: "Contacts",
                totalCount: response.Total,
                statusCode: StatusCodes.Status200OK));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    [HttpPatch("accept")]
    public async Task<IActionResult> Accept([FromBody] AcceptRequestRequestDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var updatedContact = await _contactRepository.UpdateUserRequestAsync(request, currUser);
            return Accepted(ApiResponse<AcceptRequestResponseDto>.SuccessResponse(new AcceptRequestResponseDto
                {
                    ContactId = request.ContactId,
                    IsAccepted = request.IsAccepted,
                }, "Operation success", null,
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

            var contact = await _contactRepository.BlockUserAsync(request, currUserId, currUserName);

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