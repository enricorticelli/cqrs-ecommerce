namespace Catalog.Domain.Events.Brand;

public sealed record BrandCreatedDomainEvent(Guid BrandId, string Name, string Slug, string Description);
