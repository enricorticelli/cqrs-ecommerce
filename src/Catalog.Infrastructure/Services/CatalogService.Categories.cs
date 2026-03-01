using Catalog.Application;
using Catalog.Domain;
using Marten;

namespace Catalog.Infrastructure;

public sealed partial class CatalogService
{
    public async Task<IReadOnlyList<CategoryDocument>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _querySession.Query<CategoryDocument>()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<CategoryDocument?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _querySession.LoadAsync<CategoryDocument>(id, cancellationToken);
    }

    public async Task<CategoryDocument> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = new CategoryDocument
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description
        };

        _documentSession.Store(category);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<CategoryDocument?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var existing = await _documentSession.LoadAsync<CategoryDocument>(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = new CategoryDocument
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

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _documentSession.LoadAsync<CategoryDocument>(id, cancellationToken);
        if (category is null)
        {
            return false;
        }

        _documentSession.Delete<CategoryDocument>(id);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
