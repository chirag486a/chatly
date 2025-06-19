using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IContactRepository
{
    public Task<CreateContactResponseDto> Create(CreateContactRequestDto contact, string userId, string userName);
    public Task<Contact> UpdateUserRequestAsync(AcceptRequestRequestDto request, string userId);
    public Task<Contact> BlockUserAsync(BlockRequestDto request, string userId, string currentUserName);
    public Task<GetUserContactsResponseDto> GetAllUserContactsAsync(GetUserContactsRequestDto request, string userId);
}