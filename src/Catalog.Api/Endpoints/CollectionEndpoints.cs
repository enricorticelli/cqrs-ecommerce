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

    private static async Task<Ok<IReadOnlyList<CollectionResponse>>> GetCollections(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 200, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var collections = await queryDispatcher.ExecuteAsync(new GetCollectionsQuery(safeLimit, safeOffset, searchTerm), cancellationToken);
        IReadOnlyList<CollectionResponse> response = collections.Select(CollectionMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<CollectionResponse>, NotFound>> GetCollectionById(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var collection = await queryDispatcher.ExecuteAsync(new GetCollectionByIdQuery(id), cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(CollectionMapper.ToResponse(collection));
    }

    private static async Task<Created<CollectionResponse>> CreateCollection(
        CreateCollectionRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = CollectionMapper.ToCreateCollectionCommand(request);
        var collection = await commandDispatcher.ExecuteAsync(new CreateCollectionCatalogCommand(command), cancellationToken);
        return TypedResults.Created($"/v1/collections/{collection.Id}", CollectionMapper.ToResponse(collection));
    }

    private static async Task<Results<Ok<CollectionResponse>, NotFound>> UpdateCollection(
        Guid id,
        UpdateCollectionRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = CollectionMapper.ToUpdateCollectionCommand(request);
        var collection = await commandDispatcher.ExecuteAsync(new UpdateCollectionCatalogCommand(id, command), cancellationToken);
        return collection is null ? TypedResults.NotFound() : TypedResults.Ok(CollectionMapper.ToResponse(collection));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCollection(Guid id, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var deleted = await commandDispatcher.ExecuteAsync(new DeleteCollectionCatalogCommand(id), cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
