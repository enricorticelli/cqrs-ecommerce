using Payment.Application.Abstractions;
using Payment.Application.Commands;
using Payment.Application.Models;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class AuthorizePaymentCommandHandler(IPaymentService paymentService)
    : ICommandHandler<AuthorizePaymentCommand, PaymentAuthorizationResult>
{
    public Task<PaymentAuthorizationResult> HandleAsync(AuthorizePaymentCommand command, CancellationToken cancellationToken)
    {
        return paymentService.AuthorizeAsync(command.Request, cancellationToken);
    }
}
