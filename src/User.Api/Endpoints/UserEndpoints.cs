using Microsoft.AspNetCore.Http.HttpResults;
using Shared.BuildingBlocks.Api;
using Shared.BuildingBlocks.Cqrs.Abstractions;
using User.Api.Contracts;
using User.Api.Contracts.Responses;
using User.Api.Mappers;
using User.Application.Queries;

namespace User.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(UserRoutes.Base)
            .WithTags("User")
            .AddEndpointFilter<CqrsExceptionEndpointFilter>();

        group.MapGet("/{id:guid}", GetUser)
            .WithName("GetUserById");
        return group;
    }

    private static async Task<Results<Ok<UserResponse>, NotFound>> GetUser(Guid id, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken)
    {
        var user = await queryDispatcher.ExecuteAsync(new GetUserByIdQuery(id), cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(UserMapper.ToResponse(user));
    }
}
