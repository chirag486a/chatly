using System.Net;

namespace Chatly.Exceptions;

public class ApplicationException : Exception
{
    public bool Success { get; protected set; } = false;
    public string? Message { get; protected set; }
    public Dictionary<string, List<string>>? Errors { get; protected set; } // validation or other errors
    public int? StatusCode { get; protected set; } // for richer status handling
    public string? ErrorCode { get; protected set; } // optional machine-readable error code
    public string? Details { get; protected set; } // additional error or info detail

    public ApplicationException(string? message) : base(message)
    {
    }

    public ApplicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ApplicationException<T> : ApplicationException where T : ApplicationException<T>
{
    public ApplicationException(string? message) : base(message)
    {
    }


    public static T ErrorResponse(
        string message = "Operation failed.",
        int? statusCode = StatusCodes.Status400BadRequest,
        string? errorCode = null,
        string? details = null,
        Dictionary<string, List<string>>? errors = null
    )
    {
        return (T)new ApplicationException<T>(message)
            {
                Success = false,
                StatusCode = statusCode,
                ErrorCode = errorCode,
                Errors = errors,
                Message = message,
                Details = details
            }
            ;
    }

    public virtual T AddError(string key, string error)
    {
        if (Errors == null)
            Errors = new Dictionary<string, List<string>>();

        if (!Errors.ContainsKey(key))
            Errors[key] = new List<string>();

        Errors[key].Add(error);

        return (T)this;
    }

    public virtual T SetStatusCode(int statusCode)
    {
        this.StatusCode = statusCode;
        return (T)this;
    }

    public virtual T SetErrorCode(string errorCode)
    {
        this.ErrorCode = errorCode;
        return (T)this;
    }

    public virtual T SetErrorDetails(string errorDetails)
    {
        Details = errorDetails;
        return (T)this;
    }
}