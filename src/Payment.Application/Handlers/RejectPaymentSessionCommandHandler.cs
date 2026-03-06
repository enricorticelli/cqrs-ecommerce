using Payment.Application.Abstractions;
using Payment.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Handlers;

public sealed class RejectPaymentSessionCommandHandler(IPaymentSessionService paymentSessionService)
    : ICommandHandler<RejectPaymentSessionCommand, bool>
{
    public Task<bool> HandleAsync(RejectPaymentSessionCommand command, CancellationToken cancellationToken)
    {
        return paymentSessionService.RejectSessionAsync(command.SessionId, command.Reason, cancellationToken);
    }
}
