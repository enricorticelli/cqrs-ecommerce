using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Infrastructure.Persistence.ReadModels;

namespace Catalog.Infrastructure.Services;

public sealed class BrandQueryService(CatalogReadModelStore readModelStore) : IBrandQueryService
{
    public async Task<IReadOnlyList<BrandView>> GetBrandsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var rows = await readModelStore.ListBrandsAsync(limit, offset, searchTerm, cancellationToken);
        return rows.Select(x => new BrandView(x.Id, x.Name, x.Slug, x.Description)).ToArray();
    }

    public async Task<BrandView?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await readModelStore.GetBrandByIdAsync(id, cancellationToken);
        return row is null ? null : new BrandView(row.Id, row.Name, row.Slug, row.Description);
    }
}
