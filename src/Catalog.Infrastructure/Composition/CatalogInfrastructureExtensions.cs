using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Messaging.Handlers;
using Catalog.Infrastructure.Persistence.ReadModels;
using Catalog.Infrastructure.Services;
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
            options.Discovery.IncludeType<CatalogDomainEventProjectionHandler>();
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddSingleton<CatalogReadModelStore>();
        builder.Services.AddScoped<IBrandQueryService, BrandQueryService>();
        builder.Services.AddScoped<IBrandCommandService, BrandCommandService>();
        builder.Services.AddScoped<ICategoryQueryService, CategoryQueryService>();
        builder.Services.AddScoped<ICategoryCommandService, CategoryCommandService>();
        builder.Services.AddScoped<ICollectionQueryService, CollectionQueryService>();
        builder.Services.AddScoped<ICollectionCommandService, CollectionCommandService>();
        builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
        builder.Services.AddScoped<IProductCommandService, ProductCommandService>();
        return builder;
    }
}
