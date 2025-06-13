using System.Net;

namespace Chatly.Exceptions;

public class NotFoundException : ApplicationException<NotFoundException>
{
    public int? DefaultStatusCode { get; private set; } = StatusCodes.Status404NotFound;

    public NotFoundException(string? message) : base(message)
    {
        StatusCode = DefaultStatusCode;
        ErrorCode = "NOT_FOUND";
        Message = message;
    }

    public NotFoundException(string? message, string detail) : base(message)
    {
        StatusCode = DefaultStatusCode;
        Details = detail;
        ErrorCode = "NOT_FOUND";
        Message = message;
    }
}