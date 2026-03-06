namespace Order.Domain.Events;

public sealed record OrderCompletedDomain(Guid OrderId, string TrackingCode, string TransactionId);
