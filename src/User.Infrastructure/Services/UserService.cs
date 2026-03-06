using Marten;
using User.Application.Abstractions;
using User.Application.Dtos;
using User.Domain.Aggregates;

namespace User.Infrastructure.Services;

public sealed class UserService(IQuerySession querySession) : IUserService
{
    public async Task<UserView?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await querySession.LoadAsync<UserAggregate>(id, cancellationToken);
        return user is null ? null : new UserView(user.Id, user.Email, user.FullName);
    }
}
