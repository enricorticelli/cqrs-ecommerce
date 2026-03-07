using Catalog.Api.Contracts;
using Catalog.Application.Collections;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Views;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Catalog.Api.Endpoints;

public static class CollectionEndpoints
{
    public static RouteGroupBuilder MapCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Collections)
            .WithTags("Catalog")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapGet("/", GetCollections).WithName("GetCollections");
        group.MapGet("/{id:guid}", GetCollectionById).WithName("GetCollectionById");
        group.MapPost("/", CreateCollection).WithName("CreateCollection");
        group.MapPut("/{id:guid}", UpdateCollection).WithName("UpdateCollection");
        group.MapDelete("/{id:guid}", DeleteCollection).WithName("DeleteCollection");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<CollectionView>>> GetCollections(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 200, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var collections = await queryDispatcher.ExecuteAsync(new GetCollectionsQuery(safeLimit, safeOffset), cancellationToken);
        return TypedResults.Ok(collections);
    }

    private static async Task<Results<Ok<CollectionView>, NotFound>> GetCollectionById(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var collection = await queryDispatcher.ExecuteAsync(new GetCollectionByIdQuery(id), cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(collection);
    }

    private static async Task<Created<CollectionView>> CreateCollection(
        CreateCollectionCommand command,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var collection = await commandDispatcher.ExecuteAsync(new CreateCollectionCatalogCommand(command), cancellationToken);
        return TypedResults.Created($"/v1/collections/{collection.Id}", collection);
    }

    private static async Task<Results<Ok<CollectionView>, NotFound>> UpdateCollection(
        Guid id,
        UpdateCollectionCommand command,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var collection = await commandDispatcher.ExecuteAsync(new UpdateCollectionCatalogCommand(id, command), cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(collection);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCollection(Guid id, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var deleted = await commandDispatcher.ExecuteAsync(new DeleteCollectionCatalogCommand(id), cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
