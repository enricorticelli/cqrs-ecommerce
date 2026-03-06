using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Commands;

public sealed record AuthorizePaymentSessionCommand(Guid SessionId) : ICommand<bool>;
