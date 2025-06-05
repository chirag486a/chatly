using System.Text.RegularExpressions;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Controllers;

[ApiController]
public class UserActionController : ControllerBase
{
    UserManager<User> _userManager;
    ApplicationDbContext _dbContext;

    public UserActionController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet("api/users/")]
    public async Task<IActionResult> QueryUser(UserSearchRequestDto request)
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
            var totalCount = await _userManager.Users.CountAsync();

            return Ok(
                ApiResponse<List<UserDto>>.SuccessResponse(
                    users,
                    "Some users have been found",
                    totalCount,
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
}