namespace Order.Application;

public sealed record OrderCreationResult(Guid OrderId, string Status);
