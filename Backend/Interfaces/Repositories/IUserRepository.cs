using Chatly.DTO;
using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto request);
    public Task<bool> DeleteUserAsync(string? userId = null);

    public Task<User> UpdateUserAsync(
        string? userId = null,
        string? username = null,
        string? displayName = null,
        string? theme = null
    );
}