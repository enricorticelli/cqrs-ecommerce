namespace Catalog.Domain.Events.Category;

public sealed record CategoryUpdatedDomainEvent(Guid CategoryId, string Name, string Slug, string Description);
