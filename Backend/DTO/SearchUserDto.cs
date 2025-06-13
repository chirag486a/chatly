namespace Chatly.DTO;

// ----------------------------
// 🔍 4: User Search DTOs
// ----------------------------
public class SearchUsersRequestDto
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}

public class SearchUsersResponseDto
{
    public List<UserDto>? Users { get; set; }
    public int total { get; set; } = 0;
}