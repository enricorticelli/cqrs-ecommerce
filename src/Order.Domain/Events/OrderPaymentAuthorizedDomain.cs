namespace Order.Domain.Events;

public sealed record OrderPaymentAuthorizedDomain(Guid OrderId, string TransactionId);
