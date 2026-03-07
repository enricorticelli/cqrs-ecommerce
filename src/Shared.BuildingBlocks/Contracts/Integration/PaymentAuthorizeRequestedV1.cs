namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record PaymentAuthorizeRequestedV1(Guid OrderId, Guid UserId, decimal Amount, string PaymentMethod);
