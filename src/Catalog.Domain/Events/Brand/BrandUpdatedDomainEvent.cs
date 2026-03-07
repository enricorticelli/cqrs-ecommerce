namespace Catalog.Domain.Events.Brand;

public sealed record BrandUpdatedDomainEvent(Guid BrandId, string Name, string Slug, string Description);
