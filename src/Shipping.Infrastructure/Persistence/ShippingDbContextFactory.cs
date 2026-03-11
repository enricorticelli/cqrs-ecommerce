using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shipping.Infrastructure.Persistence;

public sealed class ShippingDbContextFactory : IDesignTimeDbContextFactory<ShippingDbContext>
{
    public ShippingDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ShippingDb") 
                               ?? throw new InvalidOperationException(
                                   "Connection string not found in environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<ShippingDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ShippingDbContext(optionsBuilder.Options);
    }
}
