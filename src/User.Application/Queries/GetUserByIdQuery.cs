using Shared.BuildingBlocks.Cqrs.Abstractions;
using User.Application.Dtos;

namespace User.Application.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserView?>;
