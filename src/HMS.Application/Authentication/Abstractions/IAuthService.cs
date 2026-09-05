using HMS.Application.Authentication.Contracts;
namespace HMS.Application.Authentication.Abstractions;
public interface IAuthService
{
 Task RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
 Task<TokenResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
 Task<TokenResponse> RefreshAsync(RefreshRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
 Task LogoutAsync(Guid userId, string jwtId, CancellationToken cancellationToken = default);
 Task<AuthenticatedUser?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
