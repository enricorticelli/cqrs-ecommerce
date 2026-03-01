using Cart.Api.Endpoints;
using Cart.Application;
using Cart.Infrastructure.Composition;
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
builder.Services.AddValidatorsFromAssemblyContaining<AddCartItemCommandValidator>();

builder.AddCartInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("default");
app.UseCorrelationId();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapCartEndpoints();

await app.RunAsync();
