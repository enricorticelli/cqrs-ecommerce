using Communication.Application.Handlers;
using Microsoft.AspNetCore.Builder;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Communication.Infrastructure.Configuration;

public static class CommunicationHostBuilderExtensions
{
    public static WebApplicationBuilder AddCommunicationModule(this WebApplicationBuilder builder)
    {
        var options = CommunicationTechnicalOptions.FromConfiguration(builder.Configuration);

        builder.Services.AddCommunicationInfrastructure(builder.Configuration);

        if (!options.SkipWolverineBootstrap)
        {
            builder.Host.UseWolverine(wolverine =>
            {
                wolverine.Discovery.IncludeAssembly(typeof(SendOrderCompletedEmailHandler).Assembly);
                wolverine.UseRabbitMq(options.RabbitMqUri).AutoProvision();

                wolverine.ListenToRabbitQueue("order-completed-communication");
                wolverine.ListenToRabbitQueue("shipment-intransit-communication");

                wolverine.PersistMessagesWithPostgresql(options.CommunicationConnectionString);
                wolverine.UseEntityFrameworkCoreTransactions();
            });
        }

        return builder;
    }
}
