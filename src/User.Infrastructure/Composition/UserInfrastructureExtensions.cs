using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Infrastructure;
using User.Application.Abstractions;
using User.Infrastructure.Services;
using Wolverine;
using Wolverine.RabbitMQ;

namespace User.Infrastructure.Composition;

public static class UserInfrastructureExtensions
{
    public static WebApplicationBuilder AddUserInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(InfrastructureConnectionFactory.BuildPostgresConnectionString(
                Environment.GetEnvironmentVariable("USER_DB") ?? "userdb"));
            options.DatabaseSchemaName = "users";
        });

        builder.Host.UseWolverine(options =>
        {
            options.UseRabbitMq(InfrastructureConnectionFactory.BuildRabbitMqConnectionString());
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddScoped<IUserService, UserService>();
        return builder;
    }
}
