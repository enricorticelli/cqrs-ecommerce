using Shared.BuildingBlocks.Http;
using Warehouse.Api.Endpoints;
using Warehouse.Infrastructure.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.AddWarehouseInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("default");
app.UseCorrelationId();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapWarehouseEndpoints();

await app.RunAsync();
