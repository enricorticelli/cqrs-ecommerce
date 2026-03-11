using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shared.BuildingBlocks.Configuration;

namespace Account.Infrastructure.Persistence;

public sealed class AccountDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        var connectionString = EnvironmentVariableReader.ResolveRequired("ConnectionStrings__AccountDb");

        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AccountDbContext(optionsBuilder.Options);
    }
}
