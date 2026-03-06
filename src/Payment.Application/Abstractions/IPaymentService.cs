using Payment.Application.Models;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Payment.Application.Abstractions;

public interface IPaymentService
{
    Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizeRequestedV1 request, CancellationToken cancellationToken);
}
