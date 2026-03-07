using Catalog.Application.Views;

namespace Catalog.Application.Abstractions.Brands;

public interface IBrandService
{
    Task<IReadOnlyList<BrandView>> GetBrandsAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<BrandView> GetBrandAsync(Guid id, CancellationToken cancellationToken);
    Task<BrandView> CreateBrandAsync(string name, string slug, string description, string correlationId, CancellationToken cancellationToken);
    Task<BrandView> UpdateBrandAsync(Guid id, string name, string slug, string description, string correlationId, CancellationToken cancellationToken);
    Task DeleteBrandAsync(Guid id, string correlationId, CancellationToken cancellationToken);
}
