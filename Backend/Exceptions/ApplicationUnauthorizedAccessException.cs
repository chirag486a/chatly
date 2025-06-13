using System.Net;
using Chatly.Models;

namespace Chatly.Exceptions;

public class ApplicationUnauthorizedAccessException : ApplicationException<ApplicationUnauthorizedAccessException>
{
    public ApplicationUnauthorizedAccessException(string? message) : base(message)
    {
        Message = message;
    }

    public ApplicationUnauthorizedAccessException(string? message, string? detail) : base(message)
    {
        Message = message;
        Details = detail;
        StatusCode = StatusCodes.Status401Unauthorized;
    }
}