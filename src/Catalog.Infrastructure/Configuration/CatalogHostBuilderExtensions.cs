using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Catalog.Infrastructure.Configuration;

public static class CatalogHostBuilderExtensions
{
    public static WebApplicationBuilder AddCatalogModule(this WebApplicationBuilder builder)
    {
        var options = CatalogTechnicalOptions.FromConfiguration(builder.Configuration);

        builder.Services.AddCatalogInfrastructure(builder.Configuration);

        if (!options.SkipWolverineBootstrap)
        {
            builder.Host.UseWolverine(wolverine =>
            {
                wolverine.UseRabbitMq(options.RabbitMqUri).AutoProvision();
                wolverine.PersistMessagesWithPostgresql(options.CatalogConnectionString);
                wolverine.UseEntityFrameworkCoreTransactions();
            });
        }

        return builder;
    }
}
