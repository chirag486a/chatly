using Chatly.DTO;
using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IContactRepository
{
    public Task<SendRequestResponseDto> Create(SendRequestRequestDto contact, string userId);
    public Task<Contact> Get(string contactId);
}