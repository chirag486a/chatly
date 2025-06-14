using System.Net;
using System.Runtime.CompilerServices;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Identity;
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


    // Creates Contacts    
    [HttpPost]
    public async Task<IActionResult> SendRequest(SendRequestRequestDto request)
    {
        try
        {
            if (User.GetUserId() == null)
            {
                throw new ApplicationUnauthorizedAccessException("User not logged in", "The user id is null").AddError(
                    "UserId", "User id is null");
            }


            var newContact = await _contactRepository.Create(request, User.GetUserId());
            return CreatedAtAction(nameof(GetContact),
                new { userId = newContact.Id }, // This is the routeValues object
                ApiResponse<SendRequestResponseDto>.SuccessResponse(newContact, "Added contact", null,
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
            var contact = await _contactRepository.Get(userId);
            return Ok(ApiResponse<Contact>.SuccessResponse(contact, "Contact found", null, StatusCodes.Status200OK));
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
}