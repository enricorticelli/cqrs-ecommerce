using Shared.BuildingBlocks.Cqrs.Abstractions;
using User.Application.Abstractions;
using User.Application.Dtos;
using User.Application.Queries;

namespace User.Application.Handlers;

public sealed class GetUserByIdQueryHandler(IUserService userService)
    : IQueryHandler<GetUserByIdQuery, UserView?>
{
    public Task<UserView?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return userService.GetUserByIdAsync(query.UserId, cancellationToken);
    }
}
