using Chatly.DTO;
using Chatly.DTO.Accounts;
using Chatly.Models;

namespace Chatly.Interfaces.Services;

public interface ITokenService
{
    public TokenResult GenerateJwtToken(User user);
}