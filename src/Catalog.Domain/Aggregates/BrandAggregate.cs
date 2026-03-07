using Catalog.Domain.Events;
using Catalog.Domain.Events.Brand;

namespace Catalog.Domain.Aggregates;

public sealed class BrandAggregate
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }

    public void Apply(BrandCreatedDomainEvent @event)
    {
        Id = @event.BrandId;
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
        IsDeleted = false;
    }

    public void Apply(BrandUpdatedDomainEvent @event)
    {
        Name = @event.Name;
        Slug = @event.Slug;
        Description = @event.Description;
    }

    public void Apply(BrandDeletedDomainEvent _)
    {
        IsDeleted = true;
    }
}
