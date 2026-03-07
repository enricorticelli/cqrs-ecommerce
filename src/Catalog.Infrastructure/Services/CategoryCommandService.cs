using Catalog.Application.Abstractions;
using Catalog.Application.Categories;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events;
using Catalog.Domain.Events.Category;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class CategoryCommandService(IDocumentSession documentSession) : ICategoryCommandService
{
    public async Task<CategoryView> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryId = Guid.NewGuid();
        var @event = new CategoryCreatedDomainEvent(categoryId, command.Name, command.Slug, command.Description);

        var state = new CategoryAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<CategoryAggregate>(categoryId, @event);
        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CategoryView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<CategoryView?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return null;
        }

        var @event = new CategoryUpdatedDomainEvent(id, command.Name, command.Slug, command.Description);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CategoryView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<CategoryAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return false;
        }

        var @event = new CategoryDeletedDomainEvent(id);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
