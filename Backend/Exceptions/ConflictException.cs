using System.Net;

namespace Chatly.Exceptions;

public class ConflictException : ApplicationException<ConflictException>
{
    public int? DefaultStatusCode { get; private set; } = StatusCodes.Status409Conflict;

    public ConflictException(string message) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ErrorCode = "CONFLICT";
    }

    public ConflictException(string message, string detail) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ErrorCode = "CONFLICT";
        Details = detail;
    }
}