using System.Net;
using Chatly.Controllers;
using Chatly.Data;
using Chatly.DTO;
using Chatly.DTO.Contacts;
using Chatly.Exceptions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationException = Chatly.Exceptions.ApplicationException;

namespace Chatly.Repositories;

public class ContactRepository : IContactRepository
{
    private ApplicationDbContext _dbContext;

    public ContactRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Contact> Create(
        string? userId = null,
        string? contactUserId = null,
        string? currUserName = null,
        string? contactUserName = null)
    {
        try
        {
            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(currUserName))
            {
                throw new ApplicationArgumentException("Invalid input", nameof(userId))
                    .SetErrorDetails("User id is null or empty")
                    .AddError("UserId", "User id is required");
            }

            var currUser =
                await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId || x.UserName == currUserName);
            if (currUser == null)
            {
                throw new NotFoundException($"Couldn't create contact for user {userId}")
                    .AddError("userId", "Not found")
                    .SetErrorDetails("The current user id could not be found");
            }


            if (!string.IsNullOrEmpty(contactUserName) &&
                !string.IsNullOrEmpty(contactUserId))
            {
                throw new ApplicationArgumentException("Invalid input", nameof(contactUserId)).AddParam(
                        nameof(contactUserName))
                    .AddError("ContactUserId", "Contact user id is required if contact username is null")
                    .AddError("ContactUserName", "Contact user name is null")
                    .SetErrorDetails("Can't create contact for null user");
            }

            if (contactUserId == userId || contactUserName == currUserName)
            {
                throw new ConflictException("Couldn't create contact user")
                    .SetErrorDetails("The contact details matched the requester user")
                    .AddError("ContactUserId", $"The contact details matched the requester user id {userId}");
            }

            var contactUser = await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Id == contactUserId || u.UserName == contactUserName);

            if (contactUser == null)
            {
                throw new NotFoundException("Unable to create contact")
                    .SetErrorDetails("User does not exits")
                    .AddError("ContactUserId", "User does not exits with the user id");
            }

            var ifExists = await _dbContext.Contacts.AnyAsync(u =>
                (u.UserId == currUser.Id && u.ContactId == contactUser.Id) ||
                (u.ContactId == currUser.Id && u.UserId == contactUser.Id)
            );


            if (ifExists)
            {
                throw new ConflictException("Unable to add contact", "The contact already exits").AddError("Contact",
                    "The contact already exits");
            }


            var newContact = new Contact
            {
                Id = Guid.NewGuid().ToString(),
                ContactId = contactUser.Id,
                UserId = currUser.Id,
                Status = ContactStatus.Pending,
                CreatedAt = DateTime.Now,
                ChatDeleted = false,
                Mutated = false,
                Archived = false,
                UnreadCount = 0,
            };
            var contact = await _dbContext.Contacts.AddAsync(newContact);
            if (!(await _dbContext.SaveChangesAsync() > 0))
            {
                throw new InternalServerException("Couldn't create contact", "Unable to save changes to db").AddError(
                    "SERVER", "Database error");
            }

            return newContact;
        }
        catch (ApplicationException e)
        {
            Console.WriteLine(e.Details);
            throw;
        }
        catch (DbUpdateException e)
        {
            throw new InternalServerException("Something went wrong while saving contacts",
                e.Message).AddError(e.Source != null ? e.Source : "DB",
                e.InnerException != null ? e.InnerException.Message : e.Message);
        }
    }

    public async Task<Contact> UpdateAsync(
        string? contactId = null,
        string? userId = null,
        string? contactUserId = null,
        ContactStatus? contactStatus = null,
        bool? chatDeleted = null,
        bool? mutated = null,
        bool? archived = null,
        int? unreadCount = null
    )
    {
        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId) ??
                      throw new NotFoundException("Unable to update contact",
                          $"Contact with {contactId} as contact id does not exists");

        var contactUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == contactUserId);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        //  Checking if the provided ids(contactUserId, userId) matches the ids stored in contact
        if (
            !(string.IsNullOrEmpty(contactUserId) ||
              string.IsNullOrEmpty(userId)) &&
            !((contact.ContactId == contactId && contact.UserId == userId) ||
              (contact.ContactId == userId && contact.UserId == contactId)))
        {
            throw new ApplicationUnauthorizedAccessException("Unable to update contact",
                "The contact users does not belong to this contact");
        }


        contact.ContactId = contactUser?.Id ?? contact.ContactId;
        contact.UserId = user?.Id ?? contact.UserId;
        contact.Status = contactStatus ?? contact.Status;
        contact.ChatDeleted = chatDeleted ?? contact.ChatDeleted;
        contact.Mutated = mutated ?? contact.Mutated;
        contact.Archived = archived ?? contact.Archived;
        contact.UnreadCount = unreadCount ?? contact.UnreadCount;

        _dbContext.Contacts.Update(contact);
        await _dbContext.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact?> GetAsync(
        string? contactId = null,
        string? contactUserId = null,
        string? contactUserName = null,
        string? userId = null
    )
    {
        var error = new ApplicationArgumentException("Unable to retrieve contact", nameof(contactId));
        if (contactId == null && userId == null)
        {
            throw error.AddError("UserId", "If contact id is null userId must be provided").AddParam(nameof(userId))
                .SetErrorDetails("Could not find contact when both contact id and userid are null");
        }

        Contact? contact = null;
        contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);


        if (contact == null && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(contactUserId))
        {
            contact = await _dbContext.Contacts.FirstOrDefaultAsync(c =>
                (c.UserId == userId && c.ContactId == contactId) ||
                (c.ContactId == userId && c.UserId == contactId)
            );
        }

        if (string.IsNullOrEmpty(contactUserId))
        {
            error.AddError("ContactUserId", "Contact user id field is null but not required")
                .AddParam(nameof(contactUserId));
        }

        if (contact == null && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(contactUserName))
        {
            var contactUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == contactUserName) ??
                              throw error
                                  .AddParam(nameof(contactUserName))
                                  .AddError(
                                      "ContactUserName",
                                      "If other fields are discarded contact username is a must")
                                  .SetErrorDetails(
                                      "Could not find the contact user to get contact or contact doesnot exits");
            contact = await _dbContext.Contacts.FirstOrDefaultAsync(c =>
                (c.UserId == userId && c.ContactId == contactUser.Id) ||
                (c.ContactId == userId && c.UserId == contactUser.Id)
            );
        }

        return contact;
    }

    public async Task<(List<Contact>, int)> GetAllAsync(
        string? userId = null,
        int page = 1,
        int pageSize = 10,
        bool excludeBlocked = true,
        bool excludeNone = true,
        bool onlyBlocked = false,
        bool onlyNone = false
    )

    {
        try
        {
            var queryable = _dbContext.Contacts.Where(x =>
                (x.UserId == userId || x.ContactId == userId)
            );
            // return (queryable.ToList(), 3);
            if (onlyBlocked)
            {
                queryable = queryable.Where(x => x.Status == ContactStatus.Blocked);
            }

            if (onlyNone)
            {
                queryable = queryable.Where(x => x.Status == ContactStatus.None);
            }

            if (excludeBlocked)
            {
                queryable = queryable.Where(x => x.Status != ContactStatus.None);
            }

            if (excludeNone)
            {
                queryable = queryable.Where(x => x.Status != ContactStatus.None);
            }

            var contactsCounts = await queryable.CountAsync();
            queryable = queryable.Skip((page - 1) * pageSize).Take(pageSize);
            var contacts = await queryable.ToListAsync();


            return (contacts, contactsCounts);
        }
        catch (ArgumentNullException e)
        {
            if (e.InnerException is not null)
                throw new ApplicationArgumentException(e.InnerException.Message, e.ParamName);
            throw new ApplicationArgumentException("Invalid parameter", e.ParamName);
        }
    }
}