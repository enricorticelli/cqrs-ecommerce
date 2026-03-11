namespace Account.Infrastructure.Persistence.Entities;

public sealed class CustomerAddressEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
