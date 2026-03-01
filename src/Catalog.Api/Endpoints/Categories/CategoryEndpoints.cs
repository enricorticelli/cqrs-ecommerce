using Catalog.Application;
using Catalog.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Http;

namespace Catalog.Api.Endpoints;

public static partial class CatalogEndpoints
{
    private static void MapCategoryEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", GetCategories).WithName("GetCategories");
        group.MapGet("/{id:guid}", GetCategoryById).WithName("GetCategoryById");
        group.MapPost("/", CreateCategory).WithName("CreateCategory");
        group.MapPut("/{id:guid}", UpdateCategory).WithName("UpdateCategory");
        group.MapDelete("/{id:guid}", DeleteCategory).WithName("DeleteCategory");
    }

    private static async Task<Ok<IReadOnlyList<CategoryDocument>>> GetCategories(ICatalogService service, CancellationToken cancellationToken)
    {
        var categories = await service.GetCategoriesAsync(cancellationToken);
        return TypedResults.Ok(categories);
    }

    private static async Task<Results<Ok<CategoryDocument>, NotFound>> GetCategoryById(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var category = await service.GetCategoryByIdAsync(id, cancellationToken);
        return category is null ? TypedResults.NotFound() : TypedResults.Ok(category);
    }

    private static async Task<Results<Created<CategoryDocument>, ProblemHttpResult>> CreateCategory(
        CreateCategoryCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid category payload");
        }

        var category = await service.CreateCategoryAsync(command, cancellationToken);
        return TypedResults.Created($"/v1/categories/{category.Id}", category);
    }

    private static async Task<Results<Ok<CategoryDocument>, NotFound, ProblemHttpResult>> UpdateCategory(
        Guid id,
        UpdateCategoryCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid category payload");
        }

        var category = await service.UpdateCategoryAsync(id, command, cancellationToken);
        return category is null ? TypedResults.NotFound() : TypedResults.Ok(category);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCategory(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteCategoryAsync(id, cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
