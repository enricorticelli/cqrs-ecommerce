using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface IProductQueryService
{
    Task<IReadOnlyList<ProductView>> GetProductsAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetNewArrivalsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductView>> GetBestSellersAsync(CancellationToken cancellationToken);
    Task<ProductView?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
}
