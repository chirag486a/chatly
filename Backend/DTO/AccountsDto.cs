namespace Chatly.DTO;

public class SignupRequestDto
{
    public string? UserName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public DateTime? LastSeen { get; set; }
    public bool IsOnline { get; set; }
}

public class TokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}