using HotelLakeview.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IActionResult ToProblem(this ControllerBase controller, Result result)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return controller.ToProblem(result.Error!);
    }

    public static IActionResult ToProblem(this ControllerBase controller, ResultError error)
    {
        var (statusCode, title) = error.Type switch
        {
            ResultErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation failed"),
            ResultErrorType.NotFound => (StatusCodes.Status404NotFound, "Resource not found"),
            ResultErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ResultErrorType.InvalidOperation => (StatusCodes.Status422UnprocessableEntity, "Invalid operation"),
            _ => (StatusCodes.Status400BadRequest, "Request error"),
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = error.Message,
        };

        problem.Extensions["code"] = error.Code;

        return controller.StatusCode(statusCode, problem);
    }
}
