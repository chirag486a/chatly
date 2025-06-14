using System.Net;
using Chatly.Data;
using Chatly.DTO;
using Chatly.Exceptions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
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

    public async Task<SendRequestResponseDto> Create(SendRequestRequestDto request, string userId)
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

            return new SendRequestResponseDto()
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


    public async Task<Contact> Get(string contactId)
    {
        try
        {
            var contact = await _dbContext.Contacts.Include(x => x.User).Include(x => x.ContactUser)
                .FirstOrDefaultAsync(x => x.Id == contactId);
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
}