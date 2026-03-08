using Microsoft.AspNetCore.Builder;
using Shipping.Application.Handlers;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Order;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Shipping.Infrastructure.Configuration;

public static class ShippingHostBuilderExtensions
{
    public static WebApplicationBuilder AddShippingModule(this WebApplicationBuilder builder)
    {
        var options = ShippingTechnicalOptions.FromConfiguration(builder.Configuration);

        builder.Services.AddShippingInfrastructure(builder.Configuration);

        if (!options.SkipWolverineBootstrap)
        {
            builder.Host.UseWolverine(wolverine =>
            {
                wolverine.Discovery.IncludeAssembly(typeof(CreateShipmentOnOrderCompletedHandler).Assembly);
                wolverine.UseRabbitMq(options.RabbitMqUri).AutoProvision();

                // Incoming integration events to Shipping
                wolverine.ListenToRabbitQueue("order-completed-shipping");

                wolverine.PersistMessagesWithPostgresql(options.ShippingConnectionString);
                wolverine.UseEntityFrameworkCoreTransactions();
            });
        }

        return builder;
    }
}
