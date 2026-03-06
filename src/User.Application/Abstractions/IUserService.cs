using User.Application.Dtos;

namespace User.Application.Abstractions;

public interface IUserService
{
    Task<UserView?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
}
