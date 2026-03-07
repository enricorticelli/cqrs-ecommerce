using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Abstractions;
using Payment.Infrastructure.Messaging.Handlers;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Persistence.ReadModels;
using Payment.Infrastructure.Services;
using Shared.BuildingBlocks.Contracts.Integration;
using Shared.BuildingBlocks.Infrastructure;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Payment.Infrastructure.Composition;

public static class PaymentInfrastructureExtensions
{
    public static WebApplicationBuilder AddPaymentInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(InfrastructureConnectionFactory.BuildPostgresConnectionString(
                Environment.GetEnvironmentVariable("PAYMENT_DB") ?? "paymentdb"));
            options.DatabaseSchemaName = "payment";
        });

        builder.Host.UseWolverine(options =>
        {
            options.UseRabbitMq(InfrastructureConnectionFactory.BuildRabbitMqConnectionString())
                .AutoProvision();
            options.ListenToRabbitQueue(IntegrationQueueNames.PaymentWorkflow);
            options.PublishMessage<PaymentAuthorizedV1>().ToRabbitQueue(IntegrationQueueNames.OrderWorkflow);
            options.PublishMessage<PaymentFailedV1>().ToRabbitQueue(IntegrationQueueNames.OrderWorkflow);
            options.Discovery.IncludeType<PaymentAuthorizeRequestedHandler>();
            options.Discovery.IncludeType<PaymentDomainEventProjectionHandler>();
            options.Policies.AutoApplyTransactions();
        });

        builder.Services.AddScoped<PaymentService>();
        builder.Services.AddScoped<IPaymentReadModelStore, MongoPaymentReadModelStore>();
        builder.Services.AddScoped<IPaymentReadStore, PaymentReadStore>();
        builder.Services.AddScoped<IPaymentStateStore, MartenPaymentStateStore>();
        builder.Services.AddScoped<IPaymentService>(sp => sp.GetRequiredService<PaymentService>());
        builder.Services.AddScoped<IPaymentSessionService>(sp => sp.GetRequiredService<PaymentService>());
        return builder;
    }
}
