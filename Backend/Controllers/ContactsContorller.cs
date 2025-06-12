using Chatly.Data;
using Chatly.DTO;
using Chatly.Extensions;
using Chatly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private UserManager<User> _userManager;
    private ApplicationDbContext _dbContext;

    public ContactsController(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateContactRequestDto request)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.ContactUserId) && request.ContactUserId == User.GetUserId())
            {
                return Conflict(ApiResponse<object>.ErrorResponse(
                    "Contact user already exits",
                    409,
                    "DUPLICATE_USER",
                    "The contact already exits with this id.",
                    new Dictionary<string, List<string>>
                    {
                        {
                            "ContactUserId", new List<string> { "Contact already exits." }
                        }
                    }
                ));
            }

            if (string.IsNullOrEmpty(request.ContactUserId) ||
                (await _userManager.FindByIdAsync(request.ContactUserId) is var user && user == null))
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
            if (request.ContactUserId == currUserId)
                return BadRequest(
                    ApiResponse<object>.ErrorResponse("Can't add to contact with same person",
                        404,
                        "INVALID_INPUT",
                        "Your user id is same as the contact user id and they can't be same",
                        new Dictionary<string, List<string>>
                            { { "ContactUserId", new List<string> { "Can't be same as your id" } } })
                );

            var currUser = await _userManager.FindByIdAsync(currUserId);

            if (currUser == null)
                throw new Exception("User does not exist.");

            var newContact = new Contact
            {
                Id = Guid.NewGuid().ToString(),
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

            return CreatedAtAction(nameof(GetContact),
                ApiResponse<CreateContactResponseDto>.SuccessResponse(new CreateContactResponseDto()
                {
                    Id = newContact.Id,
                    ContactId = newContact.ContactId,
                    UserId = newContact.UserId,
                    Status = newContact.Status,
                    CreatedAt = newContact.CreatedAt,
                    ChatDeleted = newContact.ChatDeleted,
                    Mutated = newContact.Mutated,
                    Archived = newContact.Archived,
                    UnreadCount = newContact.UnreadCount,
                }, "Added contact", null, 201));
        }
        catch (Exception e)
        {
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

    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetContact([FromRoute] string userId)
    {
        throw new NotImplementedException();
    }
}