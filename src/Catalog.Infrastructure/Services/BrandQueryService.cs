using Catalog.Application.Abstractions;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class BrandQueryService(IQuerySession querySession) : IBrandQueryService
{
    public async Task<IReadOnlyList<BrandView>> GetBrandsAsync(int limit, int offset, string? searchTerm, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 200);
        var safeOffset = Math.Max(offset, 0);
        var normalizedSearch = searchTerm?.Trim();

        var query = querySession.Query<BrandAggregate>()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            var loweredSearch = normalizedSearch.ToLowerInvariant();
            query = query.Where(x =>
                x.Name.ToLower().Contains(loweredSearch) ||
                x.Slug.ToLower().Contains(loweredSearch) ||
                x.Description.ToLower().Contains(loweredSearch));
        }

        var brands = await query
            .OrderBy(x => x.Name)
            .Skip(safeOffset)
            .Take(safeLimit)
            .ToListAsync(cancellationToken);

        return brands.Select(MapToView).ToArray();
    }

    public async Task<BrandView?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var brand = await querySession.LoadAsync<BrandAggregate>(id, cancellationToken);
        return brand is null || brand.IsDeleted ? null : MapToView(brand);
    }

    private static BrandView MapToView(BrandAggregate brand)
    {
        return new BrandView(brand.Id, brand.Name, brand.Slug, brand.Description);
    }
}
