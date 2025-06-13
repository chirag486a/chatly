using System.Net;

namespace Chatly.Exceptions;

public class ApplicationArgumentException : ApplicationException<ApplicationArgumentException>
{
    public string? ParamName { get; protected set; }
    public int? DefaultStatusCode { get; protected set; } = StatusCodes.Status500InternalServerError;

    public ApplicationArgumentException(string? message, string? paramName) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ParamName = paramName;
    }

    public ApplicationArgumentException(string? message, string? paramName, string details) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ErrorCode = "INVALID_ARGUMENT";
        ParamName = paramName;
        Details = details;
    }

    public override ApplicationArgumentException SetErrorDetails(string errorDetails)
    {
        return base.SetErrorDetails(errorDetails + $" [{ParamName}]");
    }

    public virtual ApplicationArgumentException AddError(string error)
    {
        if (ParamName == null)
        {
            throw new ArgumentException("ParamName is not initialize for argument exception", nameof(ParamName));
        }

        return base.AddError(ParamName, error);
    }
}