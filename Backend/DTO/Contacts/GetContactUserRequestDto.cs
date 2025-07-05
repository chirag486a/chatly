using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatly.DTO.Accounts;

namespace Backend.DTO.Contacts;

public class GetContactUserRequestDto
{
    public string? ContactId { get; set; }
}

public class ContactUserDto
{
    public string? ContactId { get; set; }
    public UserDto? ContactUser { get; set; }

}