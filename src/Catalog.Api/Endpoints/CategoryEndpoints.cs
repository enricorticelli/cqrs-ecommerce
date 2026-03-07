using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Abstractions.Categories;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;

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

    private static async Task<IResult> GetCategories(string? searchTerm, ICategoryService service, CancellationToken cancellationToken)
    {
        var categories = await service.GetCategoriesAsync(searchTerm, cancellationToken);
        return Results.Ok(categories.Select(x => new CategoryResponse(x.Id, x.Name, x.Slug, x.Description)));
    }

    private static async Task<IResult> GetCategoryById(Guid id, ICategoryService service, CancellationToken cancellationToken)
    {
        try
        {
            var category = await service.GetCategoryAsync(id, cancellationToken);
            return Results.Ok(new CategoryResponse(category.Id, category.Name, category.Slug, category.Description));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> CreateCategory(CreateCategoryRequest request, ICategoryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var category = await service.CreateCategoryAsync(request.Name, request.Slug, request.Description, correlationId, cancellationToken);
            var response = new CategoryResponse(category.Id, category.Name, category.Slug, category.Description);
            return Results.Created($"{CatalogRoutes.Categories}/{category.Id}", response);
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryRequest request, ICategoryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var category = await service.UpdateCategoryAsync(id, request.Name, request.Slug, request.Description, correlationId, cancellationToken);
            return Results.Ok(new CategoryResponse(category.Id, category.Name, category.Slug, category.Description));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> DeleteCategory(Guid id, ICategoryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            await service.DeleteCategoryAsync(id, correlationId, cancellationToken);
            return Results.NoContent();
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }
}
