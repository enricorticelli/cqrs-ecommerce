namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderAddress(string Street, string City, string PostalCode, string Country)
{
    public static OrderAddress Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty);
}
