namespace Shared.BuildingBlocks.Contracts;

public sealed record PaymentAuthorizeRequestedV1(Guid OrderId, Guid UserId, decimal Amount);
