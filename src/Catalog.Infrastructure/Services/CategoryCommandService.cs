using Catalog.Application.Abstractions;
using Catalog.Application.Categories;
using Catalog.Application.Views;
using Catalog.Domain.Aggregates;
using Catalog.Domain.Events.Category;
using Marten;
using Wolverine;

namespace Catalog.Infrastructure.Services;

public sealed class CategoryCommandService(IDocumentSession documentSession, IMessageBus bus) : ICategoryCommandService
{
    public async Task<CategoryView> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryId = Guid.NewGuid();
        var @event = new CategoryCreatedDomainEvent(categoryId, command.Name, command.Slug, command.Description);

        var state = new CategoryAggregate();
        state.Apply(@event);

        documentSession.Events.StartStream<CategoryAggregate>(categoryId, @event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);

        return new CategoryView(state.Id, state.Name, state.Slug, state.Description);
    }

    public async Task<CategoryView?> UpdateCategoryAsync(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CategoryAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return null;
        }

        var @event = new CategoryUpdatedDomainEvent(id, command.Name, command.Slug, command.Description);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);

        return new CategoryView(id, command.Name, command.Slug, command.Description);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var stream = await documentSession.Events.FetchForWriting<CategoryAggregate>(id, cancellationToken);
        if (!stream.Events.Any() || stream.Aggregate?.IsDeleted == true)
        {
            return false;
        }

        var @event = new CategoryDeletedDomainEvent(id);
        stream.AppendOne(@event);
        await documentSession.SaveChangesAsync(cancellationToken);
        await bus.PublishAsync(@event);
        return true;
    }
}
