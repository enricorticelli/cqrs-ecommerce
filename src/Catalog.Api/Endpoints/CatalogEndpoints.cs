using Microsoft.AspNetCore.Http.HttpResults;

namespace Catalog.Api.Endpoints;

public static partial class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        MapProductEndpoints(app.MapGroup("/v1/products").WithTags("Catalog.Products"));
        MapBrandEndpoints(app.MapGroup("/v1/brands").WithTags("Catalog.Brands"));
        MapCategoryEndpoints(app.MapGroup("/v1/categories").WithTags("Catalog.Categories"));
        MapCollectionEndpoints(app.MapGroup("/v1/collections").WithTags("Catalog.Collections"));
        return app;
    }

    private static ProblemHttpResult CreateValidationProblem(Dictionary<string, string[]> errors, string detail)
    {
        return TypedResults.Problem(
            title: "Validation error",
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errors"] = errors });
    }
}
