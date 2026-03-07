using Microsoft.Extensions.Configuration;

namespace Catalog.Infrastructure.Configuration;

public sealed record CatalogTechnicalOptions(
    string CatalogConnectionString,
    Uri RabbitMqUri,
    bool SkipWolverineBootstrap)
{
    public static CatalogTechnicalOptions FromConfiguration(IConfiguration configuration)
    {
        var connectionString = ResolveCatalogConnectionString(configuration);
        var rabbitHost = configuration["MessageBus__Host"] ?? "rabbitmq";
        var rabbitPort = configuration["MessageBus__Port"] ?? "5672";
        var rabbitUsername = configuration["MessageBus__Username"] ?? "app";
        var rabbitPassword = configuration["MessageBus__Password"] ?? "app";
        var rabbitUri = new Uri($"amqp://{rabbitUsername}:{rabbitPassword}@{rabbitHost}:{rabbitPort}");
        var skipBootstrap = configuration.GetValue<bool>("Catalog:SkipWolverineBootstrap");

        return new CatalogTechnicalOptions(connectionString, rabbitUri, skipBootstrap);
    }

    public static string ResolveCatalogConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings__CatalogDb"] ?? configuration.GetConnectionString("CatalogDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Catalog connection string is missing.");
        }

        return connectionString;
    }
}
