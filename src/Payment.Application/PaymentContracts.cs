using Shared.BuildingBlocks.Contracts;

namespace Payment.Application;

public sealed record PaymentAuthorizationResult(Guid OrderId, bool Authorized, string? TransactionId = null);

public interface IPaymentService
{
    Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizeRequestedV1 request, CancellationToken cancellationToken);
}
