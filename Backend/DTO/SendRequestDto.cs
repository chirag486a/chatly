namespace Chatly.DTO;

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------
public class CreateContactRequestDto
{
    public string? ContactUserId { get; set; }
}

public class CreateContactResponseDto
{
    public string? Id { get; set; }
    public string RequestStatus { get; set; } = "Pending"; // or Accepted/Rejected
}