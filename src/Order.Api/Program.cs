using Order.Api.Endpoints;
using Order.Infrastructure.Composition;
using Shared.BuildingBlocks.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHttpClient("cart", client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CART_API_URL") ?? "http://cart-api:8080");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient("warehouse", client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("WAREHOUSE_API_URL") ?? "http://warehouse-api:8080");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient("payment", client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PAYMENT_API_URL") ?? "http://payment-api:8080");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient("shipping", client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SHIPPING_API_URL") ?? "http://shipping-api:8080");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.AddOrderInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("default");
app.UseCorrelationId();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapOrderEndpoints();

await app.RunAsync();
