using System.Net;
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

    public async Task<CreateContactResponseDto> Create(CreateContactRequestDto request, string userId)
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

            if (string.IsNullOrEmpty(request.ContactUserId) ||
                (await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.ContactUserId) is var user &&
                 user == null))
            {
                throw new NotFoundException("Unable to add users to contact").SetErrorDetails("User does not exits")
                    .AddError("ContactUserId", "User does not exits");
            }

            var currentContact = await _dbContext.Contacts.FirstOrDefaultAsync(u =>
                (u.ContactId == request.ContactUserId && u.UserId == userId) &&
                (u.ContactId == userId && u.UserId == request.ContactUserId)
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

    public async Task AcceptRequest()
    {
    }


    public async Task<List<Contact>> GetUserContacts(string userId)
    {
        try
        {
            var contact = await _dbContext.Contacts.Where(x => x.UserId == userId || x.ContactId == userId)
                .ToListAsync();
            if (contact == null)
            {
                throw new NotFoundException("Unable to get contact", "User not found in database");
            }

            return contact;
        }
        catch (ArgumentException e)
        {
            throw new ApplicationArgumentException("Argument is null or empty", e.ParamName,
                "The argument cannot be null or empty").AddError("Is null or empty");
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e.Message);
            throw new InternalServerException("Operation Cancelled", "Something went wrong in database operation");
        }
        catch (NotFoundException e)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new InternalServerException("Internal Server Error", "Something went wrong in the server");
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
                throw new NotFoundException("Unable to get contact", "Contact not found");
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

    public async Task<Contact> BlockUserAsync(BlockRequestDto request, string userId)
    {
        try
        {
            var contact = await _dbContext.Contacts.FirstOrDefaultAsync(x =>
                (x.UserId == userId || x.ContactId == userId) &&
                (x.UserId == request.ContactUserId || x.ContactId == request.ContactUserId)
            );
            if (contact == null)
            {
                var temp = await Create(new CreateContactRequestDto { ContactUserId = request.ContactUserId }, userId);
                contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == temp.Id);
            }

            if (contact == null)
            {
                throw new InternalServerException("Something went wrong", "Something went wrong");
            }

            if (contact.Status == ContactStatus.Blocked)
            {
                throw new ConflictException("Unable to block contact", "Contact is not blocked Already");
            }

            contact.Status = ContactStatus.Blocked;

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
    // update contact status
}