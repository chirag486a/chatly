using Microsoft.AspNetCore.Mvc;

namespace Chatly.Extensions;

public static class ControllerBaseExtensions
{
    private const int DefaultStatusCode = StatusCodes.Status500InternalServerError;

    public static IActionResult InternalServerError(this ControllerBase controller, object error)
    {
        return controller.StatusCode(DefaultStatusCode, error);
    }
}