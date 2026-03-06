using Catalog.Application;
using Catalog.Application.Abstractions;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.BuildingBlocks.Infrastructure;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Catalog.Infrastructure.Composition;

public static class CatalogInfrastructureExtensions
{
    public static WebApplicationBuilder AddCatalogInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(InfrastructureConnectionFactory.BuildPostgresConnectionString(
                Environment.GetEnvironmentVariable("CATALOG_DB") ?? "catalogdb"));
            options.DatabaseSchemaName = "catalog";
        });

        builder.Host.UseWolverine(options =>
        {
            options.UseRabbitMq(InfrastructureConnectionFactory.BuildRabbitMqConnectionString());
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddScoped<ICatalogService, CatalogService>();
        return builder;
    }
}
