using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;

namespace Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Categories)
            .WithTags("Catalog");

        group.MapGet("/", GetCategories).WithName("GetCategories");
        group.MapGet("/{id:guid}", GetCategoryById).WithName("GetCategoryById");
        group.MapPost("/", CreateCategory).WithName("CreateCategory");
        group.MapPut("/{id:guid}", UpdateCategory).WithName("UpdateCategory");
        group.MapDelete("/{id:guid}", DeleteCategory).WithName("DeleteCategory");

        return group;
    }

    private static IResult GetCategories()
    {
        return Results.Ok(new[] { BuildCategoryResponse(Guid.NewGuid()) });
    }

    private static IResult GetCategoryById(Guid id)
    {
        return Results.Ok(BuildCategoryResponse(id));
    }

    private static IResult CreateCategory(CreateCategoryRequest request)
    {
        var id = Guid.NewGuid();
        var response = BuildCategoryResponse(id, request.Name, request.Slug, request.Description);
        return Results.Created($"{CatalogRoutes.Categories}/{id}", response);
    }

    private static IResult UpdateCategory(Guid id, UpdateCategoryRequest request)
    {
        var response = BuildCategoryResponse(id, request.Name, request.Slug, request.Description);
        return Results.Ok(response);
    }

    private static IResult DeleteCategory(Guid id)
    {
        _ = id;
        return Results.NoContent();
    }

    private static CategoryResponse BuildCategoryResponse(Guid id, string name = "Stub category", string slug = "stub-category", string? description = "Stub description")
    {
        return new CategoryResponse(id, name, slug, description ?? string.Empty);
    }
}
