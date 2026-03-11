namespace Account.Api.Contracts.Responses;

public sealed record AddressResponse(
    Guid Id,
    string Label,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool IsDefaultShipping,
    bool IsDefaultBilling);
