namespace Chatly.DTO;

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------
public class SendRequestRequestDto
{
    public string? ContactUserId { get; set; }
}

public class SendRequestResponseDto
{
    public string RequestStatus { get; set; } = "Pending"; // or Accepted/Rejected
}