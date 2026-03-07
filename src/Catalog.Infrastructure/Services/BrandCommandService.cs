using Catalog.Application.Abstractions;
using Catalog.Application.Brands;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events.Brand;
using Marten;
using Wolverine;

namespace Catalog.Infrastructure.Services;

public sealed class BrandCommandService(IDocumentSession documentSession, IMessageBus bus) : IBrandCommandService
{
    public async Task<BrandView> CreateBrandAsync(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var brandId = Guid.NewGuid();
        var @event = new BrandCreatedDomainEvent(brandId, command.Name, command.Slug, command.Description);

        var state = new BrandAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<BrandAggregate>(brandId, @event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);

        return new BrandView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<BrandView?> UpdateBrandAsync(Guid id, UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<BrandAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return null;
        }

        var @event = new BrandUpdatedDomainEvent(id, command.Name, command.Slug, command.Description);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);

        return new BrandView(id, command.Name, command.Slug, command.Description);
    }

    public async Task<bool> DeleteBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<BrandAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return false;
        }

        var @event = new BrandDeletedDomainEvent(id);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event, cancellationToken);
        return true;
    }
}
