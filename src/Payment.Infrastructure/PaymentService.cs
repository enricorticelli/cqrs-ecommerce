using Payment.Application;
using Shared.BuildingBlocks.Contracts;
using Wolverine;

namespace Payment.Infrastructure;

public sealed class PaymentService(IMessageBus bus) : IPaymentService
{
    public async Task<PaymentAuthorizationResult> AuthorizeAsync(PaymentAuthorizeRequestedV1 request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0 || request.Amount > 10000)
        {
            await bus.PublishAsync(new PaymentFailedV1(request.OrderId, "Payment declined"));
            return new PaymentAuthorizationResult(request.OrderId, false);
        }

        var transactionId = $"TX-{Guid.NewGuid():N}";
        await bus.PublishAsync(new PaymentAuthorizedV1(request.OrderId, transactionId));
        return new PaymentAuthorizationResult(request.OrderId, true, transactionId);
    }
}
