using Microsoft.AspNetCore.Http;
using Shared.BuildingBlocks.Exceptions;

namespace Shared.BuildingBlocks.Api.Errors;

public static class ExceptionHttpResultMapper
{
    public static IResult Map(Exception exception)
    {
        return exception switch
        {
            NotFoundAppException => Results.NotFound(new { error = exception.Message }),
            ConflictAppException => Results.Conflict(new { error = exception.Message }),
            ValidationAppException => Results.BadRequest(new { error = exception.Message }),
            _ => Results.Problem(exception.Message)
        };
    }
}
