namespace Chatly.DTO.Accounts;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = String.Empty;
    public string NewPassword { get; set; } = String.Empty;
}
