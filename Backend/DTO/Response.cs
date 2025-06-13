namespace Chatly.DTO;

public class ApiResponse<T>
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }
    public int? TotalCount { get; private set; } // for paginated/list results
    public Dictionary<string, List<string>>? Errors { get; private set; } // validation or other errors
    public int? StatusCode { get; private set; } // for richer status handling
    public string? ErrorCode { get; private set; } // optional machine-readable error code
    public string? Details { get; private set; } // additional error or info detail

    public static ApiResponse<T> SuccessResponse(
        T? data = default,
        string? message = null,
        int? totalCount = null,
        int? statusCode = StatusCodes.Status202Accepted
    )
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Data = data,
            Message = message,
            TotalCount = totalCount
        };
    }

    public static ApiResponse<T> ErrorResponse(
        string? message = null,
        int? statusCode = StatusCodes.Status500InternalServerError,
        string? errorCode = null,
        string? details = null,
        Dictionary<string, List<string>>? errors = null
    )
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            ErrorCode = errorCode,
            Errors = errors,
            Message = message,
            Details = details
        };
    }

    public ApiResponse<T> AddError(string key, string error)
    {
        if (Errors == null)
            Errors = new Dictionary<string, List<string>>();

        if (!Errors.ContainsKey(key))
            Errors[key] = new List<string>();

        Errors[key].Add(error);

        return this;
    }

    public ApiResponse<T> SetStatusCode(int statusCode)
    {
        this.StatusCode = statusCode;
        return this;
    }

    public ApiResponse<T> SetErrorCode(string errorCode)
    {
        this.ErrorCode = errorCode;
        return this;
    }

    public ApiResponse<T> SetErrorDetails(string errorDetails)
    {
        this.ErrorCode = errorDetails;
        return this;
    }
}