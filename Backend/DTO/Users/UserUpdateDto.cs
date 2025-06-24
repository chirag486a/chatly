namespace Chatly.DTO.Users;

public class UserUpdateDto
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Theme { get; set; }
}

public class UserUpdateResponseDto
{
    public string Status { get; set; } = "Updated";
}