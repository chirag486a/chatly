using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chatly.DTO;
using Chatly.Interfaces.Services;
using Chatly.Models;
using Microsoft.IdentityModel.Tokens;

namespace Chatly.Services;

public class TokenService : ITokenService
{
    readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public TokenResult GenerateJwtToken(User user)
    {
        try
        {
            if (user.UserName == null)
            {
                throw new ArgumentException(nameof(user.UserName), $"{nameof(user.UserName)} cannot be null.");
            }

            var passwordKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(passwordKey))
            {
                throw new Exception("Jwt Key cannot be null.");
            }

            var expiresInMintesStrings = _config["Jwt:ExpiresInMinutes"];
            if (string.IsNullOrWhiteSpace(expiresInMintesStrings))
            {
                throw new Exception("Expires In Minutes cannot be null or empty.");
            }

            var expiresInMintes = int.Parse(expiresInMintesStrings);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(passwordKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMintes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}