using Account.Api.Endpoints;
using Account.Infrastructure.Configuration;
using Shared.BuildingBlocks.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultApiServices();
builder.AddAccountModule();

var app = builder.Build();
await app.UseAccountModuleAsync();

app.UseDefaultApiPipeline();
app.UseAuthentication();
app.UseAuthorization();
app.MapAccountEndpoints();

await app.RunAsync();
