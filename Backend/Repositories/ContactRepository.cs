using Chatly.Data;
using Chatly.Interfaces.Repositories;
using Chatly.Models;

namespace Chatly.Repositories;

public class ContactRepository : IContactRepository
{
    private ApplicationDbContext _dbContext;

    public ContactRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Contact> GetContact(string contactId)
    {
        
        var contact = _dbContext.Contacts.FirstOrDefault(x => x.Id == contactId);
        throw new NotImplementedException();
    }
}