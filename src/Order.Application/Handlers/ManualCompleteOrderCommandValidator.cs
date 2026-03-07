using Order.Application.Commands;
using Shared.BuildingBlocks.Cqrs.Pipeline;

namespace Order.Application.Handlers;

public sealed class ManualCompleteOrderCommandValidator : IRequestValidator<ManualCompleteOrderCommand>
{
    public IReadOnlyDictionary<string, string[]> Validate(ManualCompleteOrderCommand request)
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
