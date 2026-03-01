using User.Domain;

namespace User.Application;

public interface IUserService
{
    Task<UserDocument?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> SeedUsersAsync(CancellationToken cancellationToken);
}
