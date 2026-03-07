using Catalog.Application.Abstractions;
using Catalog.Application.Abstractions.Brands;
using Catalog.Application.Abstractions.Categories;
using Catalog.Application.Abstractions.Collections;
using Catalog.Application.Abstractions.Products;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services.Brands;
using Catalog.Infrastructure.Services.Categories;
using Catalog.Infrastructure.Services.Collections;
using Catalog.Infrastructure.Services.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;

namespace Catalog.Infrastructure.Configuration;

public static class CatalogInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = CatalogTechnicalOptions.ResolveCatalogConnectionString(configuration);

        services.AddDbContextWithWolverineIntegration<CatalogDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductCommandService, ProductService>();

        return services;
    }
}
