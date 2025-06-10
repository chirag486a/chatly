using System.Text.RegularExpressions;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Extensions;
using Chatly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Controllers;

[ApiController]
[Route("api/users")]
public class UserActionController : ControllerBase
{
    UserManager<User> _userManager;
    ApplicationDbContext _dbContext;

    public UserActionController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet("authtest")]
    [Authorize]
    public IActionResult secureTest()
    {
        return Ok("Hello");
    }

    [HttpGet]
    public async Task<IActionResult> QueryUser([FromQuery] UserSearchRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Query))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "No query parameter",
                    400,
                    "REQ_NULL_QUERY",
                    "The query field in the request is required but was null.",
                    new Dictionary<string, List<string>>
                    {
                        { "Query", new List<string> { "Query parameter is required." } }
                    }
                ));
            }

            if (request.Page < 0 || request.PageSize < 0)
            {
                request.Page = 0;
                request.PageSize = 5;
            }

            var query = request.Query.ToUpper();

            var users = await _userManager.Users
                .Where(u =>
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}") ||
                    u.NormalizedUserName == query
                ).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).Select(u => new UserDto
                {
                    DisplayName = u.DisplayName ?? "N/A",
                    Email = u.Email ?? "N/A",
                    Id = u.Id,
                    UserName = u.UserName ?? "N/A",
                }).ToListAsync();

            var count = await _userManager.Users
                .Where(u =>
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}") ||
                    u.NormalizedUserName == query
                ).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).Select(u => new UserDto
                {
                    DisplayName = u.DisplayName ?? "N/A",
                    Email = u.Email ?? "N/A",
                    Id = u.Id,
                    UserName = u.UserName ?? "N/A",
                }).CountAsync();

            return Ok(
                ApiResponse<List<UserDto>>.SuccessResponse(
                    users,
                    "Some users have been found",
                    count,
                    200
                ));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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

    [Authorize]
    [HttpPost("contacts")]
    public async Task<IActionResult> AddToContacts([FromBody] ContactActionDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ContactUserId) ||
                (await _userManager.FindByNameAsync(request.ContactUserId) is var user && user == null))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Could not find user",
                    404,
                    "USER_ID_NOTFOUND",
                    "User id is null or does not exist.",
                    new Dictionary<string, List<string>>
                    {
                        {
                            "ContactUserId", new List<string> { "User with id not found or user id is null" }
                        }
                    }
                ));
            }

            var currUserId = User.GetUserId();
            if (currUserId == null)
                throw new Exception("User id is null or does not exist.");

            var currUser = await _userManager.FindByIdAsync(currUserId);

            if (currUser == null)
                throw new Exception("User does not exist.");

            var newContact = new Contact
            {
                ContactId = user.Id,
                UserId = User.GetUserId(),
                Status = ContactStatus.Pending,
                CreatedAt = DateTime.Now,
                ChatDeleted = false,
                Mutated = false,
                Archived = false,
                UnreadCount = 0,
            };

            var contact = _dbContext.Contacts.Add(newContact);
            if (!(await _dbContext.SaveChangesAsync() > 0))
            {
                throw new Exception("Failed to add contact");
            }

            return CreatedAtAction(nameof(GetContacts),
                ApiResponse<Contact>.SuccessResponse(newContact, "Added contact", null, 201));
        }
        catch (Exception e)
        {
            Console.WriteLine("fkljsdfl;kajsdfl;sdkjf;lsdfjl;");
            return BadRequest(ApiResponse<object>.ErrorResponse(
                e.Message,
                500,
                "SERVER_ERROR",
                "Something went wrong in the server",
                new Dictionary<string, List<string>>
                    { { "Sever", new List<string> { "Something went wrong in the server" } } }
            ));
        }
    }

    [Authorize]
    [HttpGet("contacts")]
    public Task<IActionResult> GetContacts([FromQuery] UserSearchRequestDto request)
    {
        
        throw new NotImplementedException();
    }
}