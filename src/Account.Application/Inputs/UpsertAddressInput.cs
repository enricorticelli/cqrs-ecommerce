namespace Account.Application.Inputs;

public sealed record UpsertAddressInput(
    string Label,
    string Street,
    string City,
    string PostalCode,
    string Country,
    bool IsDefaultShipping,
    bool IsDefaultBilling);
