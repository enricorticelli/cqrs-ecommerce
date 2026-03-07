using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;

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

    private static IResult GetCollections()
    {
        return Results.Ok(new[] { BuildCollectionResponse(Guid.NewGuid()) });
    }

    private static IResult GetCollectionById(Guid id)
    {
        return Results.Ok(BuildCollectionResponse(id));
    }

    private static IResult CreateCollection(CreateCollectionRequest request)
    {
        var id = Guid.NewGuid();
        var response = BuildCollectionResponse(id, request.Name, request.Slug, request.Description, request.IsFeatured);
        return Results.Created($"{CatalogRoutes.Collections}/{id}", response);
    }

    private static IResult UpdateCollection(Guid id, UpdateCollectionRequest request)
    {
        var response = BuildCollectionResponse(id, request.Name, request.Slug, request.Description, request.IsFeatured);
        return Results.Ok(response);
    }

    private static IResult DeleteCollection(Guid id)
    {
        _ = id;
        return Results.NoContent();
    }

    private static CollectionResponse BuildCollectionResponse(
        Guid id,
        string name = "Stub collection",
        string slug = "stub-collection",
        string? description = "Stub description",
        bool isFeatured = false)
    {
        return new CollectionResponse(id, name, slug, description ?? string.Empty, isFeatured);
    }
}
