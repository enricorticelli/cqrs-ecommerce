using Order.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Pipeline;

namespace Order.Application.Handlers;

public sealed class ManualCancelOrderCommandValidator : IRequestValidator<ManualCancelOrderCommand>
{
    public IReadOnlyDictionary<string, string[]> Validate(ManualCancelOrderCommand request)
    {
        if (request.OrderId != Guid.Empty)
        {
            return new Dictionary<string, string[]>();
        }

        return new Dictionary<string, string[]>
        {
            [nameof(request.OrderId)] = ["The OrderId field must be a non-empty GUID."]
        };
    }
}
