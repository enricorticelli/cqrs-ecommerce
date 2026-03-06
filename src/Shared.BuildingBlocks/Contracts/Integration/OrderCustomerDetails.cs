namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record OrderCustomerDetails(string FirstName, string LastName, string Email, string Phone)
{
    public static OrderCustomerDetails Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty);
}
