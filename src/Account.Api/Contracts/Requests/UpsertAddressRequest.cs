namespace Account.Api.Contracts.Requests;

public sealed record UpsertAddressRequest(
    string Label,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool IsDefaultShipping,
    bool IsDefaultBilling);
