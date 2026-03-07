using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface IBrandQueryService
{
    Task<IReadOnlyList<BrandView>> GetBrandsAsync(int limit, int offset, CancellationToken cancellationToken);
    Task<BrandView?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken);
}
