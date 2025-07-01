using Chatly.Data;
using Chatly.DTO;
using Chatly.DTO.Accounts;
using Chatly.Exceptions;
using Chatly.Interfaces.Repositories;
using Chatly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApplicationException = Chatly.Exceptions.ApplicationException;

namespace Chatly.Repositories;

public class UserRepository : IUserRepository
{
    ApplicationDbContext _context;
    readonly UserManager<User> _userManager;
    IConfiguration _configuration;

    public UserRepository(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(Stream, string)> GetProfilePictureAsync(string? userId)
    {
        if (userId == null)
            throw new ApplicationArgumentException("User id cannot be null", nameof(userId));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new ApplicationArgumentException("User does not exist", nameof(userId));

        var folderPath = _configuration["Storage:UserProfilePicturesPath"];
        if (folderPath == null)
        {
            throw new Exception("Unable to resolve User Profile Pictures Storage Path");
        }


        var file = Directory
            .GetFiles(folderPath) // gets full paths
            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == userId);

        if (string.IsNullOrEmpty(file))
        {
            throw new NotFoundException("Profile picture not found",
                $"Profile Picture doesnot exits in the server for user: {userId}");
        }

        var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        return (stream, Helper.Helper.GetMimeType(file) ?? "application/octet-stream");
    }

    public async Task UpdateProfilePictureAsync(IFormFile? image, string? userId)
    {
        if (image == null) throw new ApplicationArgumentException("Image is null", nameof(image));
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extension = Path.GetExtension(image.FileName);
        if (!allowedExtensions.Contains(extension))
        {
            throw new ApplicationArgumentException(
                $"Only ->  {string.Join(", ", allowedExtensions)} are allowed", nameof(image));
        }

        if (userId == null)
            throw new ApplicationArgumentException("User id cannot be null", nameof(userId));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new ApplicationArgumentException("User does not exist", nameof(userId));


        var profilePictureDirectory = _configuration["Storage:UserProfilePicturesPath"];
        if (!Directory.Exists(profilePictureDirectory))
        {
            throw new ApplicationException("Profile picture path does not exist");
        }

        var imagePath = Path.Combine(profilePictureDirectory, user.Id) + extension;

        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }
    }

    public async Task<SearchUsersResponseDto> SearchUsers(SearchUsersRequestDto request)
    {
        try
        {
            if (request.Page < 0 || request.PageSize < 0)
            {
                request.Page = 0;
                request.PageSize = 5;
            }

            var query = request.Query.ToUpper();

            var users = await _userManager.Users.AsQueryable()
                .Where(u =>
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}") ||
                    u.NormalizedUserName == query
                ).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).Select(u => new UserDto
                {
                    DisplayName = u.DisplayName ?? "N/A",
                    Email = u.Email ?? "N/A",
                    Id = u.Id,
                    UserName = u.UserName ?? "N/A",
                }).ToListAsync();

            var count = await _userManager.Users
                .Where(u =>
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"{query}%") ||
                    EF.Functions.Like(u.NormalizedUserName, $"%{query}") ||
                    u.NormalizedUserName == query
                ).CountAsync();

            return new SearchUsersResponseDto
            {
                Users = users,
                total = count
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Something went wrong while processing the request");
        }
    }

    public async Task<bool> DeleteUserAsync(string? userId = null)
    {
        if (userId == null)
            throw new ApplicationArgumentException("UserId cannot be null", nameof(userId)).AddError(nameof(userId),
                "Cannot be null");
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("The user could not be found",
                $"User with user id : {userId} could not be found");
        }

        await _context.Contacts.Where(u => u.ContactId == user.Id || u.UserId == user.Id)
            .ExecuteDeleteAsync();
        // await _context.Messages.Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id).ExecuteDeleteAsync();
        await _userManager.DeleteAsync(user);
        return true;
    }

    public async Task<User> UpdateUserAsync(string? userId = null, string? username = null, string? displayName = null,
        string? theme = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new ApplicationArgumentException("The user could not be found", nameof(userId));
        if (!string.IsNullOrEmpty(displayName))
        {
            user.DisplayName = displayName;
        }

        if (!string.IsNullOrEmpty(theme))
        {
            user.Theme = theme;
        }

        if (!string.IsNullOrEmpty(username) && await _context.Users.AnyAsync(u => u.UserName == username))
        {
            throw new ConflictException("The user name is already taken", $"User {username} already exists");
        }

        if (!string.IsNullOrEmpty(username))
        {
            user.UserName = username;
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}