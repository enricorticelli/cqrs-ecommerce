namespace Communication.Domain.Entities;

public sealed record CommunicationEmailMessage(string Recipient, string Subject, string Body)
{
    public static CommunicationEmailMessage ForOrderCompleted(Guid orderId, decimal totalAmount, string customerEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail);

        return new CommunicationEmailMessage(
            customerEmail,
            $"Conferma ordine {orderId}",
            $"Il tuo ordine {orderId} e' stato confermato. Totale: {totalAmount:0.00}.");
    }

    public static CommunicationEmailMessage ForShipmentInTransit(Guid orderId, string trackingCode, string customerEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail);

        var normalizedTrackingCode = string.IsNullOrWhiteSpace(trackingCode) ? "n/a" : trackingCode.Trim();

        return new CommunicationEmailMessage(
            customerEmail,
            $"Ordine {orderId} spedito",
            $"Il tuo ordine {orderId} e' stato spedito. Tracking: {normalizedTrackingCode}.");
    }
}
