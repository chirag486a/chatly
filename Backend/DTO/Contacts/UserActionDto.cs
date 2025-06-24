using Chatly.Models;

namespace Chatly.DTO;





// ----------------------------
// ❌ 9: Delete User Account
// ----------------------------
public class DeleteUserDto
{
    public string PasswordConfirmation { get; set; } = string.Empty;
}

public class DeleteUserResponseDto
{
    public string Status { get; set; } = "Deleted";
}