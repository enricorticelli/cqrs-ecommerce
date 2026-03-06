namespace Order.Domain.Events;

public sealed record OrderFailedDomain(Guid OrderId, string Reason);
