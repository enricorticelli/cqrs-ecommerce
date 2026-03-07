using Catalog.Application.Abstractions;
using Catalog.Application.Collections;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events;
using Catalog.Domain.Events.Collection;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class CollectionCommandService(IDocumentSession documentSession) : ICollectionCommandService
{
    public async Task<CollectionView> CreateCollectionAsync(CreateCollectionCommand command, CancellationToken cancellationToken)
    {
        var collectionId = Guid.NewGuid();
        var @event = new CollectionCreatedDomainEvent(
            collectionId,
            command.Name,
            command.Slug,
            command.Description,
            command.IsFeatured);

        var state = new CollectionAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<CollectionAggregate>(collectionId, @event);
        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CollectionView(state.Id, state.Name, state.Slug, state.Description, state.IsFeatured);
    }

    public async Task<CollectionView?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return null;
        }

        var @event = new CollectionUpdatedDomainEvent(
            id,
            command.Name,
            command.Slug,
            command.Description,
            command.IsFeatured);

        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new CollectionView(state.Id, state.Name, state.Slug, state.Description, state.IsFeatured);
    }

    public async Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<CollectionAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return false;
        }

        var @event = new CollectionDeletedDomainEvent(id);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
