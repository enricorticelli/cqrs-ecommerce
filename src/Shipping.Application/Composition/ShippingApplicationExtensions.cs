using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Cqrs;
using Shared.BuildingBlocks.Cqrs.Dispatching;

namespace Shipping.Application.Composition;

public static class ShippingApplicationExtensions
{
    public static IServiceCollection AddShippingApplication(this IServiceCollection services)
    {
        return services.AddModuleApplication(typeof(ShippingApplicationExtensions).Assembly);
    }
}
