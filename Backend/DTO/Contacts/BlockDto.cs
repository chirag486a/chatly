namespace Chatly.DTO.Contacts;

public class BlockRequestDto
{
    public string ContactId { get; set; }
    public string ContactUserId = string.Empty;
    public string ContactUserName = string.Empty;
    public bool IsBlocked = true;
}

public class BlockResponseDto
{
    public string? ContactId = string.Empty;
}