using System.Net;

namespace Chatly.Exceptions;

public class InternalServerException : ApplicationException<InternalServerException>
{
    public InternalServerException(string message) : base(message)
    {
        Message = message;
        StatusCode = StatusCodes.Status500InternalServerError;
        ErrorCode = "SERVER_ERROR";
    }

    public InternalServerException(string message, string details) : base(message)
    {
        Message = message;
        StatusCode = StatusCodes.Status500InternalServerError;
        ErrorCode = "SERVER_ERROR";
        Details = details;
    }
}