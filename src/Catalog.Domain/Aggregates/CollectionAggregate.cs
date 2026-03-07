using Catalog.Domain.Events;
using Catalog.Domain.Events.Collection;

namespace Catalog.Domain.Aggregates;

public sealed class CollectionAggregate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsFeatured { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Apply(CollectionCreatedDomainEvent @event)
    {
        Id = @event.CollectionId;
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
        IsFeatured = @event.IsFeatured;
        IsDeleted = false;
    }

    public void Apply(CollectionUpdatedDomainEvent @event)
    {
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
        IsFeatured = @event.IsFeatured;
    }

    public void Apply(CollectionDeletedDomainEvent _)
    {
        IsDeleted = true;
    }
}
