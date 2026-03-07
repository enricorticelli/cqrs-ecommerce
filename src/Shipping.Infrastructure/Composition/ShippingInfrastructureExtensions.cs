using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Infrastructure;
using Shipping.Application.Abstractions;
using Shipping.Infrastructure.Messaging.Handlers;
using Shipping.Infrastructure.Services;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Shipping.Infrastructure.Composition;

public static class ShippingInfrastructureExtensions
{
    public static WebApplicationBuilder AddShippingInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(InfrastructureConnectionFactory.BuildPostgresConnectionString(
                Environment.GetEnvironmentVariable("SHIPPING_DB") ?? "shippingdb"));
            options.DatabaseSchemaName = "shipping";
        });

        builder.Host.UseWolverine(options =>
        {
            options.UseRabbitMq(InfrastructureConnectionFactory.BuildRabbitMqConnectionString())
                .AutoProvision();
            options.ListenToRabbitQueue(IntegrationQueueNames.ShippingWorkflow);
            options.PublishMessage<ShippingCreatedV1>().ToRabbitQueue(IntegrationQueueNames.OrderWorkflow);
            options.Discovery.IncludeType<ShippingCreateRequestedHandler>();
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddScoped<IShippingService, ShippingService>();
        return builder;
    }
}
