namespace Cart.Domain.Events;

public sealed record CartItemRemoved(Guid CartId, Guid ProductId);
