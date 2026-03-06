namespace Shared.BuildingBlocks.Contracts.Integration;

public sealed record PaymentAuthorizedV1(Guid OrderId, string TransactionId);
