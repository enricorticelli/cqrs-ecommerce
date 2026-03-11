using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Account.Infrastructure.Persistence;

public sealed class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__CommunicationDb")
                               ?? throw new InvalidOperationException(
                                   "Connection string not found in environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AccountDbContext(optionsBuilder.Options);
    }
}
