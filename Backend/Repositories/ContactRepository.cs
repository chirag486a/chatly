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

    public async Task<CreateContactResponseDto> Create(CreateContactRequestDto request, string userId, string userName)
    {
        try
        {
            var currUserId = userId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new ApplicationArgumentException("Invalid input", nameof(userId))
                    .SetErrorDetails("User id is null or empty")
                    .AddError("User id is required");
            }

            if (await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId) is var currUser && currUser == null)
            {
                throw new NotFoundException($"Couldn't create contact for user {userId}")
                    .AddError("userId", "Not found")
                    .SetErrorDetails("The current user id could not be found");
            }

            if (!string.IsNullOrEmpty(request.ContactUserId) && request.ContactUserId == userId)
            {
                throw new ConflictException("Couldn't create contact user")
                    .SetErrorDetails("The contact details matched the requester user")
                    .AddError("ContactUserId", $"The contact details matched the requester user id {userId}");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.ContactUserId);

            if (string.IsNullOrEmpty(request.ContactUserId) || user is null)
            {
                var error = new NotFoundException("Unable to add users to contact")
                    .SetErrorDetails("User does not exits")
                    .AddError("ContactUserId", "User does not exits with the user id");
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.ContactUserName) ??
                       throw error.AddError("ContactUserName", "User does not exit with the username");
            }

            var currentContact = await _dbContext.Contacts.FirstOrDefaultAsync(u =>
                (u.UserId == userId && u.ContactId == request.ContactUserId) ||
                (u.UserId == request.ContactUserId && u.ContactId == userId) ||
                (u.User.UserName == userName && u.ContactUser.UserName == request.ContactUserName) ||
                (u.User.UserName == request.ContactUserName && u.ContactUser.UserName == userName)
            );


            if (currentContact != null)
            {
                throw new ConflictException("Unable to add contact", "The contact already exits").AddError("Contact",
                    "The contact already exits");
            }

            var newContact = new Contact
            {
                Id = Guid.NewGuid().ToString(),
                ContactId = user.Id,
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

            return new CreateContactResponseDto
            {
                Id = newContact.Id,
                RequestStatus = newContact.Status.ToString(),
            };
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
    


    public async Task<Contact> UpdateUserRequestAsync(AcceptRequestRequestDto request, string userId)
    {
        try
        {
            var contact =
                await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == request.ContactId && x.ContactId == userId);
            if (contact == null)
            {
                throw new NotFoundException("Request not found",
                    "Either you are not the requester or contact does not exits").AddError("Contact",
                    "No request found");
            }

            if (contact.Status != ContactStatus.Pending)
            {
                throw new ConflictException("Unable to update contact", "Contact is not pending");
            }

            contact.Status = request.IsAccepted ? ContactStatus.Accepted : ContactStatus.None;
            _dbContext.Contacts.Update(contact);
            await _dbContext.SaveChangesAsync();
            return contact;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is not null)
                Console.WriteLine(e.InnerException.Message);
            throw new InternalServerException("Something went wrong.", "Something went wrong in database operation")
                .AddError("DB", "Failed to update contact");
        }
        catch (ApplicationException e)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new InternalServerException("Something went wrong.", "Internal server exception");
        }
    }

    public async Task<Contact> BlockUserAsync(BlockRequestDto request, string userId, string userName)
    {
        try
        {
            var contact = await _dbContext.Contacts
                .Include(u => u.ContactUser)
                .Include(u => u.User)
                .FirstOrDefaultAsync(x =>
                    ((x.UserId == userId && x.ContactId == request.ContactUserId) ||
                     (x.UserId == request.ContactUserId && x.ContactId == userId)) ||
                    (x.User.UserName == request.ContactUserName && x.ContactUser.UserName == userName) ||
                    (x.User.UserName == userName && x.ContactUser.UserName == request.ContactUserName)
                );
            if (contact == null)
            {
                var temp = await Create(
                    new CreateContactRequestDto
                        { ContactUserId = request.ContactUserId, ContactUserName = request.ContactUserName }, userId,
                    userName);
                contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == temp.Id);
            }

            Console.WriteLine(contact.User.UserName);
            Console.WriteLine(contact.ContactUser.UserName);

            if (contact == null)
            {
                throw new InternalServerException("Something went wrong", "Something went wrong");
            }

            if (contact.Status == ContactStatus.Blocked && request.IsBlocked)
            {
                throw new ConflictException("Unable to block contact", "Contact is not blocked Already");
            }

            if (contact.Status != ContactStatus.Blocked && !request.IsBlocked)
            {
                throw new ConflictException("Unable to unblock", "Contact is not not blocked");
            }


            if (request.IsBlocked)
            {
                contact.Status = ContactStatus.Blocked;
            }
            else contact.Status = ContactStatus.None;


            if (contact.UserId != userId)
            {
                contact.ContactId = contact.UserId;
                contact.UserId = userId;
            }

            _dbContext.Contacts.Update(contact);
            await _dbContext.SaveChangesAsync();
            return contact;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is not null)
                Console.WriteLine(e.InnerException.Message);
            throw new InternalServerException("Something went wrong.", "Something went wrong in database operation")
                .AddError("DB", "Failed to update contact");
        }
        catch (ApplicationException e)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new InternalServerException("Something went wrong.", "Internal server exception");
        }
    }

    public async Task<GetUserContactsResponseDto> GetAllUserContactsAsync(GetUserContactsRequestDto request,
        string userId)
    {
        try
        {
            request.PageSize = request.PageSize < 1 ? 1 : request.PageSize;
            request.Page = request.Page < 1 ? 10 : request.Page;
            var contacts = await _dbContext.Contacts.Where(x =>
                (x.UserId == userId || x.ContactId == userId) &&
                (x.Status != ContactStatus.Blocked)
            ).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
            var contactsCounts = await _dbContext.Contacts.Where(x =>
                (x.UserId == userId || x.ContactId == userId) &&
                (x.Status != ContactStatus.Blocked)
            ).CountAsync();
            return new GetUserContactsResponseDto
            {
                Contacts = contacts,
                Total = contactsCounts
            };
        }
        catch (ArgumentNullException e)
        {
            if (e.InnerException is not null)
                throw new ApplicationArgumentException(e.InnerException.Message, e.ParamName);
            throw new ApplicationArgumentException("Invalid parameter", e.ParamName);
        }
    }

    // update contact status
}