using System.Text.RegularExpressions;
using Chatly.Data;
using Chatly.DTO;
using Chatly.DTO.Accounts;
using Chatly.Exceptions;
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

    // [HttpPatch("[action]")]
    // [Authorize]
    // public async Task<IActionResult> UpdateMe()
    // {
    //     try
    //     {
    //         
    //     }
    //     catch (Exception e)
    //     {
    //         return this.InternalServerError(ApiResponse<object>.ErrorResponse(
    //             message: "Something went wrong",
    //             statusCode: StatusCodes.Status500InternalServerError,
    //             errorCode: "SERVER_ERROR"
    //         ));
    //     }
    // }


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