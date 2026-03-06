using Payment.Application.Abstractions;
using Payment.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class AuthorizePaymentSessionCommandHandler(IPaymentSessionService paymentSessionService)
    : ICommandHandler<AuthorizePaymentSessionCommand, bool>
{
    public Task<bool> HandleAsync(AuthorizePaymentSessionCommand command, CancellationToken cancellationToken)
    {
        return paymentSessionService.AuthorizeSessionAsync(command.SessionId, cancellationToken);
    }
}
