using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Api.Mappers;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Categories)
            .WithTags("Catalog")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapGet("/", GetCategories).WithName("GetCategories");
        group.MapGet("/{id:guid}", GetCategoryById).WithName("GetCategoryById");
        group.MapPost("/", CreateCategory).WithName("CreateCategory");
        group.MapPut("/{id:guid}", UpdateCategory).WithName("UpdateCategory");
        group.MapDelete("/{id:guid}", DeleteCategory).WithName("DeleteCategory");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<CategoryResponse>>> GetCategories(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 200, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var categories = await queryDispatcher.ExecuteAsync(new GetCategoriesQuery(safeLimit, safeOffset, searchTerm), cancellationToken);
        IReadOnlyList<CategoryResponse> response = categories.Select(CategoryMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<CategoryResponse>, NotFound>> GetCategoryById(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var category = await queryDispatcher.ExecuteAsync(new GetCategoryByIdQuery(id), cancellationToken);
        return category is null ? TypedResults.NotFound() : TypedResults.Ok(CategoryMapper.ToResponse(category));
    }

    private static async Task<Created<CategoryResponse>> CreateCategory(
        CreateCategoryRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = CategoryMapper.ToCreateCategoryCommand(request);
        var category = await commandDispatcher.ExecuteAsync(new CreateCategoryCatalogCommand(command), cancellationToken);
        return TypedResults.Created($"/v1/categories/{category.Id}", CategoryMapper.ToResponse(category));
    }

    private static async Task<Results<Ok<CategoryResponse>, NotFound>> UpdateCategory(
        Guid id,
        UpdateCategoryRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = CategoryMapper.ToUpdateCategoryCommand(request);
        var category = await commandDispatcher.ExecuteAsync(new UpdateCategoryCatalogCommand(id, command), cancellationToken);
        return category is null ? TypedResults.NotFound() : TypedResults.Ok(CategoryMapper.ToResponse(category));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCategory(Guid id, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var deleted = await commandDispatcher.ExecuteAsync(new DeleteCategoryCatalogCommand(id), cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
