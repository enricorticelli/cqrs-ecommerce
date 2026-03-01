using Catalog.Application;
using Catalog.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Http;

namespace Catalog.Api.Endpoints;

public static partial class CatalogEndpoints
{
    private static void MapCollectionEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", GetCollections).WithName("GetCollections");
        group.MapGet("/{id:guid}", GetCollectionById).WithName("GetCollectionById");
        group.MapPost("/", CreateCollection).WithName("CreateCollection");
        group.MapPut("/{id:guid}", UpdateCollection).WithName("UpdateCollection");
        group.MapDelete("/{id:guid}", DeleteCollection).WithName("DeleteCollection");
    }

    private static async Task<Ok<IReadOnlyList<CollectionDocument>>> GetCollections(ICatalogService service, CancellationToken cancellationToken)
    {
        var collections = await service.GetCollectionsAsync(cancellationToken);
        return TypedResults.Ok(collections);
    }

    private static async Task<Results<Ok<CollectionDocument>, NotFound>> GetCollectionById(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var collection = await service.GetCollectionByIdAsync(id, cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(collection);
    }

    private static async Task<Results<Created<CollectionDocument>, ProblemHttpResult>> CreateCollection(
        CreateCollectionCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid collection payload");
        }

        var collection = await service.CreateCollectionAsync(command, cancellationToken);
        return TypedResults.Created($"/v1/collections/{collection.Id}", collection);
    }

    private static async Task<Results<Ok<CollectionDocument>, NotFound, ProblemHttpResult>> UpdateCollection(
        Guid id,
        UpdateCollectionCommand command,
        ICatalogService service,
        CancellationToken cancellationToken)
    {
        var errors = command.GetValidationErrors();
        if (errors.Count != 0)
        {
            return CreateValidationProblem(errors, "Invalid collection payload");
        }

        var collection = await service.UpdateCollectionAsync(id, command, cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(collection);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCollection(Guid id, ICatalogService service, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteCollectionAsync(id, cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
