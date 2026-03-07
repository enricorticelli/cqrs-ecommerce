using Catalog.Api.Contracts.Requests;
using Catalog.Api.Contracts.Responses;
using Catalog.Application.Products;
using Catalog.Application.Views;

namespace Catalog.Api.Mappers;

public static class ProductMapper
{
    public static CreateProductCommand ToCreateProductCommand(CreateProductRequest request)
        => new(
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller);

    public static UpdateProductCommand ToUpdateProductCommand(UpdateProductRequest request)
        => new(
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.BrandId,
            request.CategoryId,
            request.CollectionIds,
            request.IsNewArrival,
            request.IsBestSeller);

    public static ProductResponse ToResponse(ProductView view)
        => new(
            view.Id,
            view.Sku,
            view.Name,
            view.Description,
            view.Price,
            view.BrandId,
            view.BrandName,
            view.CategoryId,
            view.CategoryName,
            view.CollectionIds,
            view.CollectionNames,
            view.IsNewArrival,
            view.IsBestSeller,
            view.CreatedAtUtc);
}
