using Chatly.DTO;

namespace Chatly.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto request);
    public Task<bool> DeleteUserAsync(string? userId = null);
}