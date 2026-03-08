using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Communication.Infrastructure.Persistence;

public sealed class CommunicationDbContextFactory : IDesignTimeDbContextFactory<CommunicationDbContext>
{
    public CommunicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CommunicationDb")
            ?? "Host=localhost;Port=5432;Database=communication_db;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<CommunicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CommunicationDbContext(optionsBuilder.Options);
    }
}
