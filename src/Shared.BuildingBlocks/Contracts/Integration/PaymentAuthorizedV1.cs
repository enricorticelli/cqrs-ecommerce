namespace Shared.BuildingBlocks.Contracts;

public sealed record PaymentAuthorizedV1(Guid OrderId, string TransactionId);
