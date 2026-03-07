using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;

namespace Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Products)
            .WithTags("Catalog");

        group.MapGet("/", GetProducts).WithName("GetProducts");
        group.MapGet("/new-arrivals", GetNewArrivals).WithName("GetNewArrivals");
        group.MapGet("/best-sellers", GetBestSellers).WithName("GetBestSellers");
        group.MapGet("/{id:guid}", GetProductById).WithName("GetProductById");
        group.MapPost("/", CreateProduct).WithName("CreateProduct");
        group.MapPut("/{id:guid}", UpdateProduct).WithName("UpdateProduct");
        group.MapDelete("/{id:guid}", DeleteProduct).WithName("DeleteProduct");

        return group;
    }

    private static IResult GetProducts()
    {
        return Results.Ok(new[] { BuildProductResponse(Guid.NewGuid()) });
    }

    private static IResult GetNewArrivals()
    {
        return Results.Ok(new[] { BuildProductResponse(Guid.NewGuid(), isNewArrival: true) });
    }

    private static IResult GetBestSellers()
    {
        return Results.Ok(new[] { BuildProductResponse(Guid.NewGuid(), isBestSeller: true) });
    }

    private static IResult GetProductById(Guid id)
    {
        return Results.Ok(BuildProductResponse(id));
    }

    private static IResult CreateProduct(CreateProductRequest request)
    {
        var id = Guid.NewGuid();
        var response = BuildProductResponse(
            id,
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller);
        return Results.Created($"{CatalogRoutes.Products}/{id}", response);
    }

    private static IResult UpdateProduct(Guid id, UpdateProductRequest request)
    {
        var response = BuildProductResponse(
            id,
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller);
        return Results.Ok(response);
    }

    private static IResult DeleteProduct(Guid id)
    {
        _ = id;
        return Results.NoContent();
    }

    private static ProductResponse BuildProductResponse(
        Guid id,
        string sku = "STUB-SKU",
        string name = "Stub product",
        string description = "Stub description",
        decimal price = 10m,
        Guid? brandId = null,
        Guid? categoryId = null,
        IReadOnlyList<Guid>? collectionIds = null,
        bool isNewArrival = false,
        bool isBestSeller = false)
    {
        var resolvedCollectionIds = collectionIds ?? Array.Empty<Guid>();
        var collectionNames = resolvedCollectionIds.Select((_, i) => $"Collection {i + 1}").ToArray();
        return new ProductResponse(
            id,
            sku,
            name,
            description,
            price,
            brandId ?? Guid.NewGuid(),
            "Stub brand",
            categoryId ?? Guid.NewGuid(),
            "Stub category",
            resolvedCollectionIds,
            collectionNames,
            isNewArrival,
            isBestSeller,
            DateTimeOffset.UtcNow);
    }
}
