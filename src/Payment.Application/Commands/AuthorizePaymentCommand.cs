using Payment.Application.Models;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Cqrs.Abstractions;

namespace Payment.Application.Commands;

public sealed record AuthorizePaymentCommand(PaymentAuthorizeRequestedV1 Request) : ICommand<PaymentAuthorizationResult>;
