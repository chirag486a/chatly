using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Models;

namespace Chatly.Interfaces.Repositories;

public interface IContactRepository
{
    public Task<Contact> Create(string? userId, string? contactUserId, string? currUserName = null,
        string? contacctUsername = null);

    public Task<Contact> UpdateAsync(
        string? contactId = null,
        string? userId = null,
        string? contactUserId = null,
        ContactStatus? contactStatus = null,
        bool? chatDeleted = null,
        bool? mutated = null,
        bool? archived = null,
        int? unreadCount = null
    );

    public Task<Contact?> GetAsync(
        string? contactId = null,
        string? contactUserId = null,
        string? contactUserName = null,
        string? userId = null
    );


    public Task<(List<Contact>, int)> GetAllAsync(
        string? userId = null,
        int page = 1,
        int pageSize = 10,
        bool excludeBlocked = true,
        bool excludeNone = true,
        bool onlyBlocked = true,
        bool onlyNone = true
    );
}