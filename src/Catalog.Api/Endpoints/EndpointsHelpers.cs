using Catalog.Api.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs;

namespace Catalog.Api.Endpoints;

public static class EndpointsHelpers
{
    public static ProblemHttpResult CreateValidationProblem(Dictionary<string, string[]> errors, string detail)
    {
        return TypedResults.Problem(
            title: "Validation error",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errors"] = errors });
    }
}
