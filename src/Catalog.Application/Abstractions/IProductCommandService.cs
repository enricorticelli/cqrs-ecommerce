using Catalog.Application.Products;
using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface IProductCommandService
{
    Task<ProductView?> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
    Task<ProductView?> UpdateProductAsync(Guid id, UpdateProductCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken cancellationToken);
}
