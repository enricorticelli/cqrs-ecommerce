namespace Order.Api.Contracts;

public sealed record OrderCreatedResponse(Guid OrderId, string Status);
