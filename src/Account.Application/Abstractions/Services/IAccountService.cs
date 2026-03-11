using Account.Application.Inputs;
using Account.Application.Models;

namespace Account.Application.Abstractions.Services;

public interface IAccountService
{
    Task<AuthTokenResult> RegisterCustomerAsync(RegisterCustomerInput request, CancellationToken cancellationToken);
    Task<AuthTokenResult> LoginAsync(string realm, LoginInput request, CancellationToken cancellationToken);
    Task<AuthTokenResult> RefreshAsync(string realm, string refreshToken, CancellationToken cancellationToken);
    Task LogoutAsync(string realm, string refreshToken, CancellationToken cancellationToken);
    Task<(bool Issued, string? PreviewCode)> CreateEmailVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken);
    Task VerifyEmailAsync(VerifyEmailInput request, CancellationToken cancellationToken);
    Task<(bool Issued, string? PreviewCode)> CreatePasswordResetCodeAsync(ForgotPasswordInput request, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordInput request, CancellationToken cancellationToken);
    Task<AccountUserModel> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    Task<AccountUserModel> UpdateProfileAsync(Guid userId, UpdateProfileInput request, CancellationToken cancellationToken);
    Task<IReadOnlyList<AccountAddressModel>> ListAddressesAsync(Guid userId, CancellationToken cancellationToken);
    Task<AccountAddressModel> CreateAddressAsync(Guid userId, UpsertAddressInput request, CancellationToken cancellationToken);
    Task<AccountAddressModel> UpdateAddressAsync(Guid userId, Guid addressId, UpsertAddressInput request, CancellationToken cancellationToken);
    Task DeleteAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderSummary>> ListMyOrdersAsync(Guid userId, CancellationToken cancellationToken);
    Task<AccountUserModel> GetAdminAsync(Guid userId, CancellationToken cancellationToken);
    Task EnsureDefaultAdminAsync(string username, string password, CancellationToken cancellationToken);
}
