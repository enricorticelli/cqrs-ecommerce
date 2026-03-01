using Microsoft.AspNetCore.Http.HttpResults;
using User.Application;
using User.Domain;

namespace User.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/users")
            .WithTags("User");

        group.MapGet("/{id:guid}", GetUser)
            .WithName("GetUserById");

        return group;
    }

    private static async Task<Results<Ok<UserDocument>, NotFound>> GetUser(Guid id, IUserService service, CancellationToken cancellationToken)
    {
        var user = await service.GetUserByIdAsync(id, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

}
