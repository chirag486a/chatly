namespace Chatly.DTO;

public class AcceptRequestRequestDto
{
    public bool IsAccepted { get; set; } = false;
}

public class AcceptRequestResponseDto
{
    public string ContactId { get; set; } = string.Empty;
}