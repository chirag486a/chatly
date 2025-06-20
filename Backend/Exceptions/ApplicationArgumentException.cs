using System.Net;

namespace Chatly.Exceptions;

public class ApplicationArgumentException : ApplicationException<ApplicationArgumentException>
{
    public List<string>? ParamName { get; protected set; }
    public int? DefaultStatusCode { get; protected set; } = StatusCodes.Status500InternalServerError;

    public ApplicationArgumentException(string? message, string paramName) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ParamName = [paramName];
    }

    public ApplicationArgumentException(string? message, string paramName, string details) : base(message)
    {
        Message = message;
        StatusCode = DefaultStatusCode;
        ErrorCode = "INVALID_ARGUMENT";
        ParamName = [paramName];
        Details = details;
    }

    public override ApplicationArgumentException SetErrorDetails(string errorDetails)
    {
        ParamName = [];
        return base.SetErrorDetails(errorDetails + $" [{string.Join(",", ParamName)}]");
    }

    public virtual ApplicationArgumentException AddParam(string key)
    {
        if (ParamName == null) ParamName = new List<string>();
        ParamName.Add(key);
        return this;
    }
}