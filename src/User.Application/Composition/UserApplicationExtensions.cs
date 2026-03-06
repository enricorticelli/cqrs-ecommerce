using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Cqrs;
using Shared.BuildingBlocks.Cqrs.Dispatching;

namespace User.Application.Composition;

public static class UserApplicationExtensions
{
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        return services.AddModuleApplication(typeof(UserApplicationExtensions).Assembly);
    }
}
