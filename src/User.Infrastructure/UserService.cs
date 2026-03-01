using Marten;
using User.Application;
using User.Domain;

namespace User.Infrastructure;

public sealed class UserService(IQuerySession querySession, IDocumentSession documentSession) : IUserService
{
    public Task<UserDocument?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return querySession.LoadAsync<UserDocument>(id, cancellationToken);
    }

    public async Task<int> SeedUsersAsync(CancellationToken cancellationToken)
    {
        var users = new[]
        {
            new UserDocument { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Email = "demo@cqrs-ecommerce.local", FullName = "Demo Customer" }
        };

        foreach (var user in users)
        {
            documentSession.Store(user);
        }

        await documentSession.SaveChangesAsync(cancellationToken);
        return users.Length;
    }
}
