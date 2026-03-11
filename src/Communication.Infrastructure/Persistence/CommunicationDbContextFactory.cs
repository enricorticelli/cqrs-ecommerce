using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Communication.Infrastructure.Persistence;

public sealed class CommunicationDbContextFactory : IDesignTimeDbContextFactory<CommunicationDbContext>
{
    public CommunicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CommunicationDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string not found in environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<CommunicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CommunicationDbContext(optionsBuilder.Options);
    }
}
