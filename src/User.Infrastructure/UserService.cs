using Marten;
using User.Application;
using User.Domain;

namespace User.Infrastructure;

public sealed class UserService(IQuerySession querySession) : IUserService
{
    public Task<UserDocument?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return querySession.LoadAsync<UserDocument>(id, cancellationToken);
    }
}
