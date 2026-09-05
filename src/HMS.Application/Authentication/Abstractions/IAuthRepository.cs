namespace HMS.Application.Authentication.Abstractions;
public sealed record AuthUser(Guid Id, string Username, string? Email, string? PasswordHash, string? PasswordSalt, bool IsActive, bool IsDeleted, DateTimeOffset? LockedUntil, int FailedLoginCount);
public sealed record RefreshTokenRecord(Guid Id, Guid UserId, Guid JwtId, byte[] TokenHash, DateTimeOffset ExpiresAt, DateTimeOffset? RevokedAt, DateTimeOffset? UsedAt);
public interface IAuthRepository
{
 Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);
 Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
 Task AddUserAsync(AuthUser user, CancellationToken cancellationToken);
 Task<AuthUser?> FindByUsernameOrEmailAsync(string value, CancellationToken cancellationToken);
 Task<AuthUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
 Task<IReadOnlyCollection<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);
 Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken);
 Task RecordLoginAsync(Guid? userId, bool successful, string? reason, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
 Task UpdateLoginStateAsync(Guid userId, bool successful, CancellationToken cancellationToken);
 Task AddTokensAsync(Guid userId, IssuedToken accessToken, byte[] accessHash, string refreshToken, byte[] refreshHash, DateTimeOffset refreshExpiresAt, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
 Task<RefreshTokenRecord?> FindRefreshTokenAsync(byte[] tokenHash, CancellationToken cancellationToken);
 Task RotateRefreshTokenAsync(RefreshTokenRecord token, IssuedToken accessToken, byte[] accessHash, string refreshToken, byte[] refreshHash, DateTimeOffset refreshExpiresAt, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
 Task RevokeAccessTokenAsync(Guid userId, Guid jwtId, CancellationToken cancellationToken);
}
