namespace Chatly.DTO.Contacts;

// ----------------------------
// ➕ 7: Send Friend/Follow Request
// ----------------------------
public class SendRequestRequestDto
{
    public string? ContactUserId { get; set; }
    public string? ContactUserName { get; set; }
}

public class SendRequestResponseDto
{
    public string? Id { get; set; }
    public string RequestStatus { get; set; } = "Pending"; // or Accepted/Rejected
}