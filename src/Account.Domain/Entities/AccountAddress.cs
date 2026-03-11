using Shared.BuildingBlocks.Exceptions;

namespace Account.Domain.Entities;

public sealed class AccountAddress
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Label { get; private set; }
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public bool IsDefaultShipping { get; private set; }
    public bool IsDefaultBilling { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }

    private AccountAddress(
        Guid id,
        Guid userId,
        string label,
        string street,
        string city,
        string postalCode,
        string country,
        bool isDefaultShipping,
        bool isDefaultBilling,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        UserId = userId;
        Label = label;
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
        IsDefaultShipping = isDefaultShipping;
        IsDefaultBilling = isDefaultBilling;
        CreatedAtUtc = createdAtUtc;
    }

    public static AccountAddress Create(
        Guid userId,
        string label,
        string street,
        string city,
        string postalCode,
        string country,
        bool isDefaultShipping,
        bool isDefaultBilling)
    {
        if (userId == Guid.Empty) throw new ValidationAppException("User id is required.");
        EnsureNotBlank(label, "Address label is required.");
        EnsureNotBlank(street, "Address street is required.");
        EnsureNotBlank(city, "Address city is required.");
        EnsureNotBlank(postalCode, "Address postal code is required.");
        EnsureNotBlank(country, "Address country is required.");

        return new AccountAddress(
            Guid.NewGuid(),
            userId,
            label.Trim(),
            street.Trim(),
            city.Trim(),
            postalCode.Trim(),
            country.Trim(),
            isDefaultShipping,
            isDefaultBilling,
            DateTimeOffset.UtcNow);
    }

    public void Update(
        string label,
        string street,
        string city,
        string postalCode,
        string country,
        bool isDefaultShipping,
        bool isDefaultBilling)
    {
        EnsureNotBlank(label, "Address label is required.");
        EnsureNotBlank(street, "Address street is required.");
        EnsureNotBlank(city, "Address city is required.");
        EnsureNotBlank(postalCode, "Address postal code is required.");
        EnsureNotBlank(country, "Address country is required.");

        Label = label.Trim();
        Street = street.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
        IsDefaultShipping = isDefaultShipping;
        IsDefaultBilling = isDefaultBilling;
    }

    public static AccountAddress Restore(
        Guid id,
        Guid userId,
        string label,
        string street,
        string city,
        string postalCode,
        string country,
        bool isDefaultShipping,
        bool isDefaultBilling,
        DateTimeOffset createdAtUtc)
    {
        return new AccountAddress(
            id,
            userId,
            label,
            street,
            city,
            postalCode,
            country,
            isDefaultShipping,
            isDefaultBilling,
            createdAtUtc);
    }

    private static void EnsureNotBlank(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationAppException(message);
        }
    }
}
