using Microsoft.AspNetCore.Builder;
using Warehouse.Application.Handlers;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Warehouse;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Warehouse.Infrastructure.Configuration;

public static class WarehouseHostBuilderExtensions
{
    public static WebApplicationBuilder AddWarehouseModule(this WebApplicationBuilder builder)
    {
        var options = WarehouseTechnicalOptions.FromConfiguration(builder.Configuration);

        builder.Services.AddWarehouseInfrastructure(builder.Configuration);

        if (!options.SkipWolverineBootstrap)
        {
            builder.Host.UseWolverine(wolverine =>
            {
                wolverine.Discovery.IncludeAssembly(typeof(ReserveStockOnOrderCreatedHandler).Assembly);
                wolverine.UseRabbitMq(options.RabbitMqUri).AutoProvision();

                // Incoming integration events to Warehouse
                wolverine.ListenToRabbitQueue("order-created-warehouse");

                // Outgoing integration events from Warehouse
                wolverine.PublishMessage<StockReservedV1>().ToRabbitQueue("stock-reserved-order");
                wolverine.PublishMessage<StockRejectedV1>().ToRabbitQueue("stock-rejected-order");

                wolverine.PersistMessagesWithPostgresql(options.WarehouseConnectionString);
                wolverine.UseEntityFrameworkCoreTransactions();
            });
        }

        return builder;
    }
}
