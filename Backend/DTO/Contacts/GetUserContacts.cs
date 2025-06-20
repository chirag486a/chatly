using Chatly.Models;

namespace Chatly.DTO.Contacts;

public class GetUserContactsRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
