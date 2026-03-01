using Shared.BuildingBlocks.Contracts;

namespace Payment.Application;

public interface IPaymentService
{
    Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizeRequestedV1 request, CancellationToken cancellationToken);
}
