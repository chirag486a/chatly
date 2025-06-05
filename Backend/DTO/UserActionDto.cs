namespace Chatly.DTO;

// ----------------------------
// 🔍 4: User Search DTOs
// ----------------------------
public class UserSearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}

// ----------------------------
// 👤 5 & 6: Contact Deletion / Blocking
// ----------------------------
public class ContactActionDto
{
    public Guid ContactUserId { get; set; }
}

public class ContactActionResponseDto
{
    public string Status { get; set; } = "Success";
}

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------
public class SendRequestDto
{
    public Guid ContactUserId { get; set; }
}

public class SendRequestResponseDto
{
    public string RequestStatus { get; set; } = "Pending"; // or Accepted/Rejected
}

// ----------------------------
// ⚙️ 8: Update Profile Info
// ----------------------------
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