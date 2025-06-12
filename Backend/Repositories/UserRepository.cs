using Chatly.Data;
using Chatly.DTO;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chatly.Repositories;

public class UserRepository : IUserRepository
{
    ApplicationDbContext _context;
    readonly UserManager<User> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto request)
    {
        try
        {
            if (request.Page < 0 || request.PageSize < 0)
            {
                request.Page = 0;
                request.PageSize = 5;
            }

            var query = request.Query.ToUpper();
            Console.WriteLine(query);

            var users = await _userManager.Users.AsQueryable()
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
                ).CountAsync();

            return new SearchUsersResponseDto
            {
                Users = users,
                total = count
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Something went wrong while processing the request");
        }
    }
}