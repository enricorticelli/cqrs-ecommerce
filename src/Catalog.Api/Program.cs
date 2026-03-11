using Catalog.Api.Endpoints;
using Catalog.Infrastructure.Configuration;
using Shared.BuildingBlocks.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddDefaultApiServices();
builder.AddAdminApiAuthentication();
builder.AddCatalogModule();

var app = builder.Build();
await app.UseCatalogModuleAsync();

app.UseDefaultApiPipeline();
app.UseAuthentication();
app.UseAuthorization();

app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapBrandEndpoints();
app.MapCollectionEndpoints();

await app.RunAsync();
