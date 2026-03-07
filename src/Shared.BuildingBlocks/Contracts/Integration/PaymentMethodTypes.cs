namespace Shared.BuildingBlocks.Contracts.Integration;

public static class PaymentMethodTypes
{
    public const string StripeCard = "stripe_card";
    public const string PayPal = "paypal";
    public const string Satispay = "satispay";

    public static readonly string[] All = [StripeCard, PayPal, Satispay];

    public static bool IsSupported(string? paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            return false;
        }

        var normalized = paymentMethod.Trim().ToLowerInvariant();
        return Array.Exists(All, method => string.Equals(method, normalized, StringComparison.Ordinal));
    }
}
