namespace Catalog.Domain.Aggregates;

public sealed class BrandAggregate
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public string Description { get; init; } = string.Empty;
}
