namespace User.Application.Dtos;

public sealed record UserView(Guid Id, string Email, string FullName);
