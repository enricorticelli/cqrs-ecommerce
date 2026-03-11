using Shared.BuildingBlocks.Api;
using Warehouse.Infrastructure.Configuration;
using Warehouse.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultApiServices();
builder.AddAdminAuthentication();
builder.AddWarehouseModule();

var app = builder.Build();
await app.UseWarehouseModuleAsync();

app.UseDefaultApiPipeline();
app.UseAuthentication();
app.UseAuthorization();
app.MapWarehouseEndpoints();

await app.RunAsync();
