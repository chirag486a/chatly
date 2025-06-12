using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IContactRepository
{
    public Task<Contact> GetContact(string contactId);
}