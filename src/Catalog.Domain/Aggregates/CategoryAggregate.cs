using Catalog.Domain.Events;
using Catalog.Domain.Events.Category;

namespace Catalog.Domain.Aggregates;

public sealed class CategoryAggregate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }

    public void Apply(CategoryCreatedDomainEvent @event)
    {
        Id = @event.CategoryId;
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
        IsDeleted = false;
    }

    public void Apply(CategoryUpdatedDomainEvent @event)
    {
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
    }

    public void Apply(CategoryDeletedDomainEvent _)
    {
        IsDeleted = true;
    }
}
