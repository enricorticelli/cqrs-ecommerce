using Catalog.Application.Abstractions;
using Catalog.Application.Collections;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events.Collection;
using Marten;
using Wolverine;

namespace Catalog.Infrastructure.Services;

public sealed class CollectionCommandService(IDocumentSession documentSession, IMessageBus bus) : ICollectionCommandService
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
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);

        return new CollectionView(state.Id, state.Name, state.Slug, state.Description, state.IsFeatured);
    }

    public async Task<CollectionView?> UpdateCollectionAsync(Guid id, UpdateCollectionCommand command, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CollectionAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return null;
        }

        var @event = new CollectionUpdatedDomainEvent(
            id,
            command.Name,
            command.Slug,
            command.Description,
            command.IsFeatured);

        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);

        return new CollectionView(id, command.Name, command.Slug, command.Description, command.IsFeatured);
    }

    public async Task<bool> DeleteCollectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CollectionAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return false;
        }

        var @event = new CollectionDeletedDomainEvent(id);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);
        return true;
    }
}
