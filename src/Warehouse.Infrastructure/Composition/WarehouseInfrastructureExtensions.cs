using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Infrastructure;
using Warehouse.Application.Abstractions;
using Warehouse.Infrastructure.Messaging.Handlers;
using Warehouse.Infrastructure.Services;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Warehouse.Infrastructure.Composition;

public static class WarehouseInfrastructureExtensions
{
    public static WebApplicationBuilder AddWarehouseInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(InfrastructureConnectionFactory.BuildPostgresConnectionString(
                Environment.GetEnvironmentVariable("WAREHOUSE_DB") ?? "warehousedb"));
            options.DatabaseSchemaName = "warehouse";
        });

        builder.Host.UseWolverine(options =>
        {
            options.UseRabbitMq(InfrastructureConnectionFactory.BuildRabbitMqConnectionString())
                .AutoProvision();
            options.ListenToRabbitQueue(IntegrationQueueNames.WarehouseWorkflow);
            options.PublishMessage<StockReservedV1>().ToRabbitQueue(IntegrationQueueNames.OrderWorkflow);
            options.PublishMessage<StockRejectedV1>().ToRabbitQueue(IntegrationQueueNames.OrderWorkflow);
            options.Discovery.IncludeType<OrderPlacedHandler>();
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
        return builder;
    }
}
