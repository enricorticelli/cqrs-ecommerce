namespace Catalog.Domain.Events.Collection;

public sealed record CollectionCreatedDomainEvent(Guid CollectionId, string Name, string Slug, string Description, bool IsFeatured);
