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

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(CatalogRoutes.Products)
            .WithTags("Catalog")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapGet("/", GetProducts).WithName("GetProducts");
        group.MapGet("/new-arrivals", GetNewArrivals).WithName("GetNewArrivals");
        group.MapGet("/best-sellers", GetBestSellers).WithName("GetBestSellers");
        group.MapGet("/{id:guid}", GetProductById).WithName("GetProductById");
        group.MapPost("/", CreateProduct).WithName("CreateProduct");
        group.MapPut("/{id:guid}", UpdateProduct).WithName("UpdateProduct");
        group.MapDelete("/{id:guid}", DeleteProduct).WithName("DeleteProduct");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> GetProducts(
        IQueryDispatcher queryDispatcher,
        int? limit,
        int? offset,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit ?? 200, 1, 200);
        var safeOffset = Math.Max(offset ?? 0, 0);
        var products = await queryDispatcher.ExecuteAsync(new GetProductsQuery(safeLimit, safeOffset, searchTerm), cancellationToken);
        IReadOnlyList<ProductResponse> response = products.Select(ProductMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> GetNewArrivals(IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var products = await queryDispatcher.ExecuteAsync(new GetNewArrivalsQuery(), cancellationToken);
        IReadOnlyList<ProductResponse> response = products.Select(ProductMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Ok<IReadOnlyList<ProductResponse>>> GetBestSellers(IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var products = await queryDispatcher.ExecuteAsync(new GetBestSellersQuery(), cancellationToken);
        IReadOnlyList<ProductResponse> response = products.Select(ProductMapper.ToResponse).ToList();
        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> GetProductById(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var product = await queryDispatcher.ExecuteAsync(new GetProductByIdQuery(id), cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(ProductMapper.ToResponse(product));
    }

    private static async Task<Results<Created<ProductResponse>, ProblemHttpResult>> CreateProduct(
        CreateProductRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = ProductMapper.ToCreateProductCommand(request);
        var product = await commandDispatcher.ExecuteAsync(new CreateProductCatalogCommand(command), cancellationToken);
        if (product is null)
        {
            return TypedResults.Problem(
                title: "Invalid product references",
                detail: "Brand, category or collection reference does not exist.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return TypedResults.Created($"/v1/products/{product.Id}", ProductMapper.ToResponse(product));
    }

    private static async Task<Results<Ok<ProductResponse>, NotFound>> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        ICommandDispatcher commandDispatcher,
        CancellationToken cancellationToken)
    {
        var command = ProductMapper.ToUpdateProductCommand(request);
        var product = await commandDispatcher.ExecuteAsync(new UpdateProductCatalogCommand(id, command), cancellationToken);
        return product is null ? TypedResults.NotFound() : TypedResults.Ok(ProductMapper.ToResponse(product));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProduct(Guid id, ICommandDispatcher commandDispatcher, CancellationToken cancellationToken)
    {
        var deleted = await commandDispatcher.ExecuteAsync(new DeleteProductCatalogCommand(id), cancellationToken);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
