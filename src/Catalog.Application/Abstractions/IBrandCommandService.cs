using Catalog.Application.Brands;
using Catalog.Application.Views;

namespace Catalog.Application.Abstractions;

public interface IBrandCommandService
{
    Task<BrandView> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken);
    Task<BrandView?> UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken);
    Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken);
}
