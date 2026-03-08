using Cart.Application.Handlers;
using Microsoft.AspNetCore.Builder;
using Shared.BuildingBlocks.Contracts.IntegrationEvents.Catalog;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Cart.Infrastructure.Configuration;

public static class CartHostBuilderExtensions
{
    public static WebApplicationBuilder AddCartModule(this WebApplicationBuilder builder)
    {
        var options = CartTechnicalOptions.FromConfiguration(builder.Configuration);

        builder.Services.AddCartInfrastructure(builder.Configuration);

        if (!options.SkipWolverineBootstrap)
        {
            builder.Host.UseWolverine(wolverine =>
            {
                wolverine.Discovery.IncludeAssembly(typeof(SyncCartOnProductUpdatedHandler).Assembly);
                wolverine.UseRabbitMq(options.RabbitMqUri).AutoProvision();

                wolverine.ListenToRabbitQueue("catalog-product-updated-cart");
                wolverine.ListenToRabbitQueue("order-completed-cart");

                // Explicit routes for cart-related catalog events
                wolverine.PublishMessage<ProductUpdatedV1>().ToRabbitQueue("catalog-product-updated-cart");
                wolverine.PublishMessage<ProductDeletedV1>().ToRabbitQueue("catalog-product-updated-cart");

                wolverine.PersistMessagesWithPostgresql(options.CartConnectionString);
                wolverine.UseEntityFrameworkCoreTransactions();
            });
        }

        return builder;
    }
}
