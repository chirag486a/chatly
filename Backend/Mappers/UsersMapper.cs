using Chatly.DTO.Accounts;
using Chatly.Models;

namespace Backend.Mappers;


public static class UsersMapper
{
  public static UserDto ToUserDtoFromUser(this User data)
  {
    return new UserDto
    {
      Id = data.Id,
      DisplayName = data.DisplayName,
      UserName = data.UserName,
      Email = data.Email,
      Theme = data.Theme,
      LastSeen = data.LastSeen,
      IsOnline = data.IsOnline
    };
  }

}