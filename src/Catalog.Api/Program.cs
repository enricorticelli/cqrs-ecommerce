using Catalog.Api.Endpoints;
using Catalog.Application;
using Catalog.Infrastructure.Composition;
using FluentValidation;
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
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

builder.AddCatalogInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("default");
app.UseCorrelationId();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapCatalogEndpoints();

await app.RunAsync();
