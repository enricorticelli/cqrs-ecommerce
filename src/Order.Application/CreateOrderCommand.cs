using System.ComponentModel.DataAnnotations;
using Order.Application.Views;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Order.Application;

public sealed record CreateOrderCommand(
    Guid CartId,
    Guid UserId,
    string IdentityType,
    string PaymentMethod,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalAmount,
    Guid? AuthenticatedUserId,
    Guid? AnonymousId,
    OrderCustomerDetails? Customer,
    OrderAddress? ShippingAddress,
    OrderAddress? BillingAddress) : ICommand<OrderCreationResult?>, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CartId == Guid.Empty)
        {
            yield return new ValidationResult("The CartId field must be a non-empty GUID.", [nameof(CartId)]);
        }

        if (UserId == Guid.Empty)
        {
            yield return new ValidationResult("The UserId field must be a non-empty GUID.", [nameof(UserId)]);
        }

        if (string.IsNullOrWhiteSpace(IdentityType))
        {
            yield return new ValidationResult("IdentityType is required.", [nameof(IdentityType)]);
        }
        else if (string.Equals(IdentityType, OrderIdentityTypes.Anonymous, StringComparison.OrdinalIgnoreCase))
        {
            if (!AnonymousId.HasValue || AnonymousId.Value == Guid.Empty)
            {
                yield return new ValidationResult("AnonymousId is required for anonymous orders.", [nameof(AnonymousId)]);
            }
        }
        else if (string.Equals(IdentityType, OrderIdentityTypes.Registered, StringComparison.OrdinalIgnoreCase))
        {
            if (!AuthenticatedUserId.HasValue || AuthenticatedUserId.Value == Guid.Empty)
            {
                yield return new ValidationResult("AuthenticatedUserId is required for registered orders.", [nameof(AuthenticatedUserId)]);
            }
        }
        else
        {
            yield return new ValidationResult(
                $"IdentityType must be '{OrderIdentityTypes.Anonymous}' or '{OrderIdentityTypes.Registered}'.",
                [nameof(IdentityType)]);
        }

        if (!PaymentMethodTypes.IsSupported(PaymentMethod))
        {
            yield return new ValidationResult(
                $"PaymentMethod must be one of: {string.Join(", ", PaymentMethodTypes.All)}.",
                [nameof(PaymentMethod)]);
        }

        if (Items is null || Items.Count == 0)
        {
            yield return new ValidationResult("At least one order item is required.", [nameof(Items)]);
        }

        if (TotalAmount <= 0)
        {
            yield return new ValidationResult("TotalAmount must be greater than zero.", [nameof(TotalAmount)]);
        }

        if (Customer is null)
        {
            yield return new ValidationResult("Customer details are required.", [nameof(Customer)]);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(Customer.FirstName))
            {
                yield return new ValidationResult("Customer first name is required.", [$"{nameof(Customer)}.{nameof(Customer.FirstName)}"]);
            }

            if (string.IsNullOrWhiteSpace(Customer.LastName))
            {
                yield return new ValidationResult("Customer last name is required.", [$"{nameof(Customer)}.{nameof(Customer.LastName)}"]);
            }

            if (string.IsNullOrWhiteSpace(Customer.Email))
            {
                yield return new ValidationResult("Customer email is required.", [$"{nameof(Customer)}.{nameof(Customer.Email)}"]);
            }
        }

        if (ShippingAddress is null)
        {
            yield return new ValidationResult("Shipping address is required.", [nameof(ShippingAddress)]);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(ShippingAddress.Street))
            {
                yield return new ValidationResult("Shipping street is required.", [$"{nameof(ShippingAddress)}.{nameof(ShippingAddress.Street)}"]);
            }

            if (string.IsNullOrWhiteSpace(ShippingAddress.City))
            {
                yield return new ValidationResult("Shipping city is required.", [$"{nameof(ShippingAddress)}.{nameof(ShippingAddress.City)}"]);
            }

            if (string.IsNullOrWhiteSpace(ShippingAddress.PostalCode))
            {
                yield return new ValidationResult("Shipping postal code is required.", [$"{nameof(ShippingAddress)}.{nameof(ShippingAddress.PostalCode)}"]);
            }

            if (string.IsNullOrWhiteSpace(ShippingAddress.Country))
            {
                yield return new ValidationResult("Shipping country is required.", [$"{nameof(ShippingAddress)}.{nameof(ShippingAddress.Country)}"]);
            }
        }

        if (BillingAddress is null)
        {
            yield return new ValidationResult("Billing address is required.", [nameof(BillingAddress)]);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(BillingAddress.Street))
            {
                yield return new ValidationResult("Billing street is required.", [$"{nameof(BillingAddress)}.{nameof(BillingAddress.Street)}"]);
            }

            if (string.IsNullOrWhiteSpace(BillingAddress.City))
            {
                yield return new ValidationResult("Billing city is required.", [$"{nameof(BillingAddress)}.{nameof(BillingAddress.City)}"]);
            }

            if (string.IsNullOrWhiteSpace(BillingAddress.PostalCode))
            {
                yield return new ValidationResult("Billing postal code is required.", [$"{nameof(BillingAddress)}.{nameof(BillingAddress.PostalCode)}"]);
            }

            if (string.IsNullOrWhiteSpace(BillingAddress.Country))
            {
                yield return new ValidationResult("Billing country is required.", [$"{nameof(BillingAddress)}.{nameof(BillingAddress.Country)}"]);
            }
        }
    }
}
