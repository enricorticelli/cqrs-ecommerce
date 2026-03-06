using Payment.Application.Abstractions;
using Shared.BuildingBlocks.Contracts.Integration;

namespace Payment.Infrastructure.Messaging.Handlers;

public sealed class PaymentAuthorizeRequestedHandler
{
    public static async Task Handle(PaymentAuthorizeRequestedV1 message, IPaymentService paymentService, CancellationToken cancellationToken)
    {
        await paymentService.AuthorizeAsync(message, cancellationToken);
    }
}
