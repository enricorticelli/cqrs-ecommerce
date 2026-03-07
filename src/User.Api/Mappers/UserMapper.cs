using User.Api.Contracts.Responses;
using User.Application.Dtos;

namespace User.Api.Mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(UserView user)
        => new(user.Id, user.Email, user.FullName);
}
