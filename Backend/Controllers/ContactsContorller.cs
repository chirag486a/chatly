using System.Net;
using System.Runtime.CompilerServices;
using Chatly.Data;
using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
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
            if (currUserId == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null").AddError(
                    "UserId", "User id is null");
            }


            var newContact = await _contactRepository.Create(request, currUserId);
            return CreatedAtAction(nameof(GetContact),
                new { userId = newContact.Id }, // This is the routeValues object
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
    public async Task<IActionResult> GetContacts()
    {
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetContact([FromRoute] string userId)
    {
        try
        {
            var contact = await _contactRepository.GetUserContacts(userId);
            return Ok(ApiResponse<List<Contact>>.SuccessResponse(contact, "Contact found", null,
                StatusCodes.Status200OK));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ApplicationException e)
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
    public async Task<IActionResult> Block([FromBody] BlockRequestDto request)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null");
            }

            var contact = await _contactRepository.BlockUserAsync(request, currUser);

            return Accepted(ApiResponse<BlockResponseDto>.SuccessResponse(new BlockResponseDto
                {
                    ContactId = request.ContactUserId,
                }, "Blocked successfully", null,
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
}