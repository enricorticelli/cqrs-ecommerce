namespace Cart.Api.Contracts;

public sealed record RemoveCartItemResponse(Guid CartId, Guid ProductId, string Message);
