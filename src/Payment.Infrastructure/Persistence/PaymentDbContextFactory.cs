using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Payment.Infrastructure.Persistence;

public sealed class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PaymentDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string not found in environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PaymentDbContext(optionsBuilder.Options);
    }
}
