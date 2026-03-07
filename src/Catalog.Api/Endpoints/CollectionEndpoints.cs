using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Abstractions.Collections;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;

namespace Catalog.Api.Endpoints;

public static class CollectionEndpoints
{
    public static RouteGroupBuilder MapCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Collections)
            .WithTags("Catalog");

        group.MapGet("/", GetCollections).WithName("GetCollections");
        group.MapGet("/{id:guid}", GetCollectionById).WithName("GetCollectionById");
        group.MapPost("/", CreateCollection).WithName("CreateCollection");
        group.MapPut("/{id:guid}", UpdateCollection).WithName("UpdateCollection");
        group.MapDelete("/{id:guid}", DeleteCollection).WithName("DeleteCollection");

        return group;
    }

    private static async Task<IResult> GetCollections(string? searchTerm, ICollectionService service, CancellationToken cancellationToken)
    {
        var collections = await service.GetCollectionsAsync(searchTerm, cancellationToken);
        return Results.Ok(collections.Select(x => new CollectionResponse(x.Id, x.Name, x.Slug, x.Description, x.IsFeatured)));
    }

    private static async Task<IResult> GetCollectionById(Guid id, ICollectionService service, CancellationToken cancellationToken)
    {
        try
        {
            var collection = await service.GetCollectionAsync(id, cancellationToken);
            return Results.Ok(new CollectionResponse(collection.Id, collection.Name, collection.Slug, collection.Description, collection.IsFeatured));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> CreateCollection(CreateCollectionRequest request, ICollectionService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var collection = await service.CreateCollectionAsync(
                request.Name,
                request.Slug,
                request.Description,
                request.IsFeatured,
                correlationId,
                cancellationToken);

            var response = new CollectionResponse(collection.Id, collection.Name, collection.Slug, collection.Description, collection.IsFeatured);
            return Results.Created($"{CatalogRoutes.Collections}/{collection.Id}", response);
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> UpdateCollection(Guid id, UpdateCollectionRequest request, ICollectionService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var collection = await service.UpdateCollectionAsync(
                id,
                request.Name,
                request.Slug,
                request.Description,
                request.IsFeatured,
                correlationId,
                cancellationToken);

            return Results.Ok(new CollectionResponse(collection.Id, collection.Name, collection.Slug, collection.Description, collection.IsFeatured));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> DeleteCollection(Guid id, ICollectionService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            await service.DeleteCollectionAsync(id, correlationId, cancellationToken);
            return Results.NoContent();
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }
}
