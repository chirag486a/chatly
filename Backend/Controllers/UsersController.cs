using System.Net;
using Chatly.DTO;
using Chatly.DTO.Accounts;
using Chatly.DTO.Users;
using Chatly.Exceptions;
using Chatly.Extensions;
using Chatly.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chatly.Helper;

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

    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> ProfilePicture()
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("You are not logged in");
            }

            var (stream, mime) = await _userRepository.GetProfilePictureAsync(currUser);

            return File(stream, mime);
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse(e.Message, StatusCodes.Status401Unauthorized,
                e.ErrorCode, e.Details, e.Errors));
        }
        catch (NotFoundException e)
        {
            return NotFound(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
    }

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile? image)
    {
        try
        {
            var currUser = User.GetUserId();
            if (currUser == null)
            {
                throw new ApplicationUnauthorizedAccessException("You are not logged in");
            }

            if (image == null || image.Length == 0)
            {
                throw new ApplicationArgumentException("Please provide an image", nameof(image));
            }

            await _userRepository.UpdateProfilePictureAsync(image, currUser);
            return Accepted(ApiResponse<object>.SuccessResponse(null, "Profile Picture Updated", null,
                StatusCodes.Status202Accepted));
        }
        catch (ApplicationUnauthorizedAccessException e)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse(e.Message, StatusCodes.Status401Unauthorized,
                e.ErrorCode, e.Details, e.Errors));
        }
    }


    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> UpdateMe(UserUpdateDto request)
    {
        try
        {
            var user = await _userRepository.UpdateUserAsync(userId: User.GetUserId(), request.Username,
                request.DisplayName,
                request.Theme);
            return Accepted(ApiResponse<UserDto>.SuccessResponse(
                new UserDto
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    Theme = user.Theme,
                    IsOnline = user.IsOnline,
                    LastSeen = user.LastSeen,
                    Email = user.Email,
                }
            ));
        }
        catch (ApplicationArgumentException e)
        {
            return this.InternalServerError(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (ConflictException e)
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse(e.Message, e.StatusCode, e.ErrorCode, e.Details, e.Errors));
        }
        catch (Exception e)
        {
            return this.InternalServerError(ApiResponse<object>.ErrorResponse(
                message: "Something went wrong",
                statusCode: StatusCodes.Status500InternalServerError,
                errorCode: "SERVER_ERROR"
            ));
        }
    }


    [HttpDelete("[action]")]
    [Authorize]
    public async Task<IActionResult> DeleteMe()
    {
        try
        {
            var currUserId = User.GetUserId();
            if (currUserId == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Unauthorized User",
                        StatusCodes.Status401Unauthorized,
                        "UNAUTHORIZED", "The user id seems to be invalid please login in again to perform action")
                    .AddError("Access", "Unauthorized Access")
                );
            var isDeleted = await _userRepository.DeleteUserAsync(currUserId);
            if (!isDeleted)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Unable to delete the user",
                    StatusCodes.Status400BadRequest, "BAD_REQUEST", $"User {currUserId} could not be deleted"));
            }

            return NoContent();
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
        catch (Exception e)
        {
            return this.InternalServerError(ApiResponse<object>.ErrorResponse("Something went wrong in server",
                StatusCodes.Status500InternalServerError, "SERVER_ERROR", "Something went wrong while deleting users"));
        }
    }
}