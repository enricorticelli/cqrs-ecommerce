using Catalog.Application.Commands;
using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface IProductCommandService
{
    Task<ProductView> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
}
