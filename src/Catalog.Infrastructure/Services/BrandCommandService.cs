using Catalog.Application.Abstractions;
using Catalog.Application.Brands;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events;
using Catalog.Domain.Events.Brand;
using Marten;

namespace Catalog.Infrastructure.Services;

public sealed class BrandCommandService(IDocumentSession documentSession) : IBrandCommandService
{
    public async Task<BrandView> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brandId = Guid.NewGuid();
        var @event = new BrandCreatedDomainEvent(brandId, command.Name, command.Slug, command.Description);

        var state = new BrandAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<BrandAggregate>(brandId, @event);
        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new BrandView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<BrandView?> UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<BrandAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return null;
        }

        var @event = new BrandUpdatedDomainEvent(id, command.Name, command.Slug, command.Description);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new BrandView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var state = await documentSession.LoadAsync<BrandAggregate>(id, cancellationToken);
        if (state is null || state.IsDeleted)
        {
            return false;
        }

        var @event = new BrandDeletedDomainEvent(id);
        documentSession.Events.Append(id, @event);
        state.Apply(@event);

        documentSession.Store(state);
        await documentSession.SaveChangesAsync(cancellationToken);
        return true;
    }
}
