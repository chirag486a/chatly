using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Chatly.DTO.Accounts;
using Chatly.DTO.Contacts;
using Chatly.Models;

namespace Backend.Mappers
{
    public static class ContactsMapper
    {
        public static ContactDto ToContactsDtoFromContact(this Contact data)
        {
            return new ContactDto
            {
                Id = data.Id,

                UserId = data.UserId,
                User = data.User?.ToUserDtoFromUser(),

                ContactId = data.ContactId,
                ContactUser = data.ContactUser?.ToUserDtoFromUser(),

                Status = data.Status.ToString(),
                CreatedAt = data.CreatedAt,
                Mutated = data.Mutated,
                Archived = data.Archived,
                UnreadCount = data.UnreadCount

            };
        }
    }
}