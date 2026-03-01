using Catalog.Application;
using Catalog.Domain;
using Marten;

namespace Catalog.Infrastructure;

public sealed partial class CatalogService
{
    public async Task<IReadOnlyList<BrandDocument>> GetBrandsAsync(CancellationToken cancellationToken)
    {
        return await _querySession.Query<BrandDocument>()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<BrandDocument?> GetBrandByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _querySession.LoadAsync<BrandDocument>(id, cancellationToken);
    }

    public async Task<BrandDocument> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = new BrandDocument
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        _documentSession.Store(brand);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return brand;
    }

    public async Task<BrandDocument?> UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        var existing = await _documentSession.LoadAsync<BrandDocument>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new BrandDocument
        {
            Id = id,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        _documentSession.Store(updated);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var brand = await _documentSession.LoadAsync<BrandDocument>(id, cancellationToken);
        if (brand is null)
        {
            return false;
        }

        _documentSession.Delete<BrandDocument>(id);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
