using System.Text.RegularExpressions;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> Search([FromQuery] SearchUsersRequestDto request)
    {
        try
        {
            var result = await _userRepository.SearchUsers(request);

            return Ok(ApiResponse<List<UserDto>?>.SuccessResponse(result.Users, "Success", result.total, 200));
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Something went wrong",
                500,
                "SERVER_ERROR",
                "Something went wrong while processing your request.",
                new Dictionary<string, List<string>>
                {
                    { "Server", new List<string> { "Something went wrong" } }
                }
            ));
        }
    }

    // [Authorize]
    // [HttpPost("contacts")]
    // public async Task<IActionResult> AddToContacts([FromBody] ContactActionDto request)
    // {
    //     try
    //     {
    //         if (!string.IsNullOrEmpty(request.ContactUserId) && request.ContactUserId == User.GetUserId())
    //         {
    //             return Conflict(ApiResponse<object>.ErrorResponse(
    //                 "Contact user already exits",
    //                 409,
    //                 "DUPLICATE_USER",
    //                 "The contact already exits with this id.",
    //                 new Dictionary<string, List<string>>
    //                 {
    //                     {
    //                         "ContactUserId", new List<string> { "Contact already exits." }
    //                     }
    //                 }
    //             ));
    //         }
    //
    //         if (string.IsNullOrEmpty(request.ContactUserId) ||
    //             (await _userManager.FindByIdAsync(request.ContactUserId) is var user && user == null))
    //         {
    //             return BadRequest(ApiResponse<object>.ErrorResponse(
    //                 "Could not find user",
    //                 404,
    //                 "USER_ID_NOTFOUND",
    //                 "User id is null or does not exist.",
    //                 new Dictionary<string, List<string>>
    //                 {
    //                     {
    //                         "ContactUserId", new List<string> { "User with id not found or user id is null" }
    //                     }
    //                 }
    //             ));
    //         }
    //
    //         var currUserId = User.GetUserId();
    //         if (currUserId == null)
    //             throw new Exception("User id is null or does not exist.");
    //         if (request.ContactUserId == currUserId)
    //             return BadRequest(
    //                 ApiResponse<object>.ErrorResponse("Can't add to contact with same person",
    //                     404,
    //                     "INVALID_INPUT",
    //                     "Your user id is same as the contact user id and they can't be same",
    //                     new Dictionary<string, List<string>>
    //                         { { "ContactUserId", new List<string> { "Can't be same as your id" } } })
    //             );
    //
    //         var currUser = await _userManager.FindByIdAsync(currUserId);
    //
    //         if (currUser == null)
    //             throw new Exception("User does not exist.");
    //
    //         var newContact = new Contact
    //         {
    //             Id = Guid.NewGuid().ToString(),
    //             ContactId = user.Id,
    //             UserId = User.GetUserId(),
    //             Status = ContactStatus.Pending,
    //             CreatedAt = DateTime.Now,
    //             ChatDeleted = false,
    //             Mutated = false,
    //             Archived = false,
    //             UnreadCount = 0,
    //         };
    //
    //         var contact = _dbContext.Contacts.Add(newContact);
    //         if (!(await _dbContext.SaveChangesAsync() > 0))
    //         {
    //             throw new Exception("Failed to add contact");
    //         }
    //
    //         return CreatedAtAction(nameof(GetContacts),
    //             ApiResponse<ContactActionResponseDto>.SuccessResponse(new ContactActionResponseDto
    //             {
    //                 Id = newContact.Id,
    //                 ContactId = newContact.ContactId,
    //                 UserId = newContact.UserId,
    //                 Status = newContact.Status,
    //                 CreatedAt = newContact.CreatedAt,
    //                 ChatDeleted = newContact.ChatDeleted,
    //                 Mutated = newContact.Mutated,
    //                 Archived = newContact.Archived,
    //                 UnreadCount = newContact.UnreadCount,
    //             }, "Added contact", null, 201));
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(ApiResponse<object>.ErrorResponse(
    //             e.Message,
    //             500,
    //             "SERVER_ERROR",
    //             "Something went wrong in the server",
    //             new Dictionary<string, List<string>>
    //                 { { "Sever", new List<string> { "Something went wrong in the server" } } }
    //         ));
    //     }
    // }

    // [Authorize]
    // [HttpGet("contacts")]
    // public Task<IActionResult> GetContacts([FromQuery] UserSearchRequestDto request)
    // {
    //     throw new NotImplementedException();
    // }
}