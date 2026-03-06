using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Commands;

public sealed record RejectPaymentSessionCommand(Guid SessionId, string Reason) : ICommand<bool>;
