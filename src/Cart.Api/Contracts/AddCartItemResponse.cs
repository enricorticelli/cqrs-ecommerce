namespace Cart.Api.Contracts;

public sealed record AddCartItemResponse(Guid CartId, string Message);
