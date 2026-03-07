namespace Catalog.Domain.Events.Collection;

public sealed record CollectionUpdatedDomainEvent(Guid CollectionId, string Name, string Slug, string Description, bool IsFeatured);
