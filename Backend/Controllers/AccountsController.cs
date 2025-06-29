using System.ComponentModel.DataAnnotations;
using Chatly.DTO;
using Chatly.DTO.Accounts;
using Chatly.Extensions;
using Chatly.Interfaces.Services;
using Chatly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatly.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IPasswordFormatValidator _passwordValidator;
    private readonly ITokenService _tokenService;

    public AccountsController(UserManager<User> userManager, IPasswordFormatValidator passwordValidator,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _passwordValidator = passwordValidator;
        _tokenService = tokenService;
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> Signup([FromBody] SignupRequestDto request)
    {
        try
        {
            Console.WriteLine("Signing up...");
            ApiResponse<object> errorResponse = ApiResponse<object>.ErrorResponse(
                "Invalid input data.",
                400,
                "INVALID_INPUT",
                "One or more fields are missing or invalid."
            );
            //  INPUT VALIDATION
            if (string.IsNullOrEmpty(request.Email))
                errorResponse.AddError("Email", "Email is required.");

            if (!string.IsNullOrEmpty(request.Email) && !new EmailAddressAttribute().IsValid(request.Email))
            {
                errorResponse.AddError("Email", "Email is invalid.");
            }

            if (request.Email != null && await _userManager.FindByEmailAsync(request.Email) != null)
            {
                errorResponse
                    .AddError("Email", "Email is already in use.")
                    .SetErrorCode("CONFLICT")
                    .SetStatusCode(409);
            }


            if (!_passwordValidator.IsValid(request.Password, out var passwordErrors))
            {
                foreach (var errors in passwordErrors)
                {
                    errorResponse.AddError("Password", errors);
                }
            }


            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrEmpty(request.UserName))
            {
                errorResponse.AddError("UserName", "UserName is required");
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                var checkUsername = await _userManager.FindByNameAsync(request.UserName);
                if (checkUsername != null)
                {
                    errorResponse
                        .AddError("UserName", "UserName is already in use.")
                        .SetErrorCode("CONFLICT")
                        .SetStatusCode(409);
                }
            }

            if (string.IsNullOrEmpty(request.DisplayName) || string.IsNullOrWhiteSpace(request.DisplayName))
            {
                errorResponse.AddError("DisplayName", "Display name is required");
            }


            if (request.Password == null || request.Email == null || request.UserName == null ||
                request.DisplayName == null || errorResponse.Errors != null && errorResponse.Errors.Any())
            {
                if (errorResponse.StatusCode == 409)
                {
                    return Conflict(errorResponse);
                }

                return BadRequest(errorResponse);
            }

            var newUser = new User
            {
                Email = request.Email,
                DisplayName = request.DisplayName,
                UserName = request.UserName
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);


            foreach (var error in result.Errors)
            {
                errorResponse.AddError(error.Code, error.Description);
            }

            if (!result.Succeeded || result.Errors.Any())
            {
                if (errorResponse.StatusCode == 409)
                {
                    return Conflict(errorResponse);
                }

                return BadRequest(errorResponse);
            }

            var user = await _userManager.FindByEmailAsync(newUser.Email);
            if (user == null)
            {
                throw new Exception("Something went wrong.");
            }

            // BUG:
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var res = await _userManager.ConfirmEmailAsync(user, token);


            var tokenResult = _tokenService.GenerateJwtToken(user);

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                new AuthResponseDto
                {
                    Token = tokenResult.Token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        DisplayName = newUser.DisplayName,
                        Email = newUser.Email,
                        UserName = newUser.UserName,
                        Theme = "system",
                        LastSeen = null,
                        IsOnline = true
                    },
                    ExpiresAt = tokenResult.ExpiresAt
                }
                , "User Created", null, 201));
        }
        catch (Exception e)
        {
            var errorResponse = ApiResponse<object>.ErrorResponse(
                "An error occured.",
                500,
                "SERVER",
                "Something went wrong"
            );
            errorResponse.AddError("Exception", e.Message);
            return BadRequest(errorResponse);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (
            string.IsNullOrEmpty(request.Email) ||
            !(new EmailAddressAttribute().IsValid(request.Email)) ||
            (await _userManager.FindByEmailAsync(request.Email) is var user && user == null)
        )
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(
                message: "Login failed. Please check your input.",
                statusCode: 400,
                errorCode: "INVALID_EMAIL",
                details: "The provided email is either invalid in format or not registered.",
                errors: new Dictionary<string, List<string>>
                {
                    { "Email", new List<string> { "The email address is invalid or does not exist." } }
                }
            ));
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(
                ApiResponse<AuthResponseDto>.ErrorResponse(
                    message: "Login failed. Please check your input.",
                    statusCode: 400,
                    errorCode: "EMPTY_PASSWORD",
                    details: "The password field cannot be empty.",
                    errors: new Dictionary<string, List<string>>
                    {
                        { "Password", new List<string> { "Password is required." } }
                    }
                )
            );
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isValidPassword)
        {
            return Unauthorized(
                ApiResponse<AuthResponseDto>.ErrorResponse(
                    message: "Login failed. Invalid email or password.",
                    statusCode: 401, // Unauthorized
                    errorCode: "INVALID_CREDENTIALS",
                    details: "The provided credentials are incorrect. Please try again.",
                    errors: new Dictionary<string, List<string>>
                    {
                        { "Email", new List<string> { "Invalid email or password." } },
                        { "Password", new List<string> { "Invalid email or password." } }
                    }
                )
            );
        }

        var tokenResult = _tokenService.GenerateJwtToken(user);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
            new AuthResponseDto
            {
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt,
                User = new UserDto
                {
                    DisplayName = user.DisplayName ?? "Anonymous",
                    Email = user.Email ?? "anonymous@gmail.com",
                    IsOnline = user.IsOnline,
                    Id = user.Id,
                    LastSeen = user.LastSeen,
                    Theme = user.Theme ?? "system",
                    UserName = user.UserName ?? "anonymous",
                }
            }
        ));
    }

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        if (
            string.IsNullOrEmpty(request.OldPassword) ||
            string.IsNullOrEmpty(request.NewPassword)
        )
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid password provided.",
                statusCode: StatusCodes.Status400BadRequest, errorCode: "INVALID_PASSWORD",
                details: "The provided password is invalid."));
        }

        if (
            (User.GetUserId() is var userId && userId == null) ||
            (await _userManager.FindByIdAsync(userId) is var user && user == null)
        )
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Unauthorized user",
                    statusCode: StatusCodes.Status401Unauthorized, errorCode: "UNAUTHORIZED",
                    "The user is not allowed to change your password")
            );
        }

        var result = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        if (!result)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Unauthorized user.",
                    statusCode: StatusCodes.Status401Unauthorized, errorCode: "UNAUTHORIZED",
                    "The user is not allowed to change your password.")
            );
        }

        await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Your password has been changed.", null,
            StatusCodes.Status200OK));
    }

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto request)
    {
        if (
            string.IsNullOrEmpty(request.NewEmail) ||
            string.IsNullOrEmpty(request.Password)
        )
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid password provided.",
                statusCode: StatusCodes.Status400BadRequest, errorCode: "INVALID_FIELDS",
                details: "The provided password is invalid."));
        }


        if (
            (User.GetUserId() is var userId && userId == null) ||
            (await _userManager.FindByIdAsync(userId) is var user && user == null)
        )
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Unauthorized user",
                    statusCode: StatusCodes.Status401Unauthorized, errorCode: "UNAUTHORIZED",
                    "The user is not allowed to change your password")
            );
        }

        if (
            await _userManager.FindByEmailAsync(request.NewEmail) is var newEmailUser &&
            newEmailUser != null &&
            newEmailUser.Id != user.Id
        )
        {
            return Conflict(
                ApiResponse<object>.ErrorResponse("Email already taken", statusCode: StatusCodes.Status403Forbidden,
                    errorCode: "EMAIL_ALREADY_TAKEN",
                    details: $"The email address '{request.NewEmail}' is already associated with another account"
                ).AddError("Email", "Already in use")
            );
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            return Unauthorized(
                ApiResponse<object>.ErrorResponse("Unauthorized user.",
                    statusCode: StatusCodes.Status401Unauthorized, errorCode: "UNAUTHORIZED",
                    "The user is not allowed to change your password.")
            );
        }

        // BUG:
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        await _userManager.ChangeEmailAsync(user, request.NewEmail, token);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Your email has been changed.", null,
            StatusCodes.Status200OK));
    }
}