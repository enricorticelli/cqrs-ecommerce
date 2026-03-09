using Microsoft.AspNetCore.Http;
using Shared.BuildingBlocks.Exceptions;

namespace Shared.BuildingBlocks.Api.Errors;

public static class ExceptionHttpResultMapper
{
    public static IResult Map(Exception exception)
    {
        return exception switch
        {
            NotFoundAppException => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: exception.Message),
            ConflictAppException => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Operation conflict",
                detail: exception.Message),
            ValidationAppException => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed",
                detail: exception.Message),
            _ => Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected error",
                detail: exception.Message)
        };
    }
}
