namespace Account.Application.Models;

public sealed record AccountAddressModel(
    Guid Id,
    Guid UserId,
    string Label,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool IsDefaultShipping,
    bool IsDefaultBilling);
