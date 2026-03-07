namespace Catalog.Domain.Events.Category;

public sealed record CategoryCreatedDomainEvent(Guid CategoryId, string Name, string Slug, string Description);
