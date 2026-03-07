using Catalog.Api.Contracts;
using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Abstractions.Products;
using Catalog.Application.Views;
using Shared.BuildingBlocks.Api.Correlation;
using Shared.BuildingBlocks.Api.Errors;

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

    private static async Task<IResult> GetProducts(string? searchTerm, IProductService service, CancellationToken cancellationToken)
    {
        var products = await service.GetProductsAsync(searchTerm, cancellationToken);
        return Results.Ok(products.Select(MapProductResponse));
    }

    private static async Task<IResult> GetNewArrivals(string? searchTerm, IProductService service, CancellationToken cancellationToken)
    {
        var products = await service.GetNewArrivalsAsync(searchTerm, cancellationToken);
        return Results.Ok(products.Select(MapProductResponse));
    }

    private static async Task<IResult> GetBestSellers(string? searchTerm, IProductService service, CancellationToken cancellationToken)
    {
        var products = await service.GetBestSellersAsync(searchTerm, cancellationToken);
        return Results.Ok(products.Select(MapProductResponse));
    }

    private static async Task<IResult> GetProductById(Guid id, IProductService service, CancellationToken cancellationToken)
    {
        try
        {
            var product = await service.GetProductAsync(id, cancellationToken);
            return Results.Ok(MapProductResponse(product));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> CreateProduct(CreateProductRequest request, IProductService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var product = await service.CreateProductAsync(
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller,
            correlationId,
            cancellationToken);

            return Results.Created($"{CatalogRoutes.Products}/{product.Id}", MapProductResponse(product));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> UpdateProduct(Guid id, UpdateProductRequest request, IProductService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            var product = await service.UpdateProductAsync(
            id,
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller,
            correlationId,
            cancellationToken);

            return Results.Ok(MapProductResponse(product));
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static async Task<IResult> DeleteProduct(Guid id, IProductService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = CorrelationIdResolver.Resolve(httpContext);
            await service.DeleteProductAsync(id, correlationId, cancellationToken);
            return Results.NoContent();
        }
        catch (Exception exception)
        {
            return ExceptionHttpResultMapper.Map(exception);
        }
    }

    private static ProductResponse MapProductResponse(ProductView product)
    {
        return new ProductResponse(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.BrandId,
            product.BrandName,
            product.CategoryId,
            product.CategoryName,
            product.CollectionIds,
            product.CollectionNames,
            product.IsNewArrival,
            product.IsBestSeller,
            product.CreatedAtUtc);
    }
}
