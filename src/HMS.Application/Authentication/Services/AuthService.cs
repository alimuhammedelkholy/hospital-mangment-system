using HMS.Application.Authentication.Abstractions;
using HMS.Application.Authentication.Contracts;
using HMS.Application.Abstractions.Persistence;
using HMS.Domain.Common.Exceptions;

namespace HMS.Application.Authentication.Services;
public sealed class AuthService(IAuthRepository repository, IPasswordHasher passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork) : IAuthService
{
 public async Task RegisterAsync(RegisterUserRequest request, CancellationToken ct = default)
 {
  if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)) throw new DomainException("Username and password are required.");
  if (await repository.UsernameExistsAsync(request.Username, ct)) throw new DomainException("Username is already registered.");
  if (!string.IsNullOrWhiteSpace(request.Email) && await repository.EmailExistsAsync(request.Email, ct)) throw new DomainException("Email is already registered.");
  var credentials = passwordHasher.Hash(request.Password);
  await repository.AddUserAsync(new AuthUser(Guid.NewGuid(), request.Username.Trim(), request.Email?.Trim(), request.Phone?.Trim(), credentials.Hash, credentials.Salt, true, false, null, 0), ct);
  await unitOfWork.SaveChangesAsync(ct);
 }
 public async Task<TokenResponse> LoginAsync(LoginRequest request, string? ip, string? ua, CancellationToken ct = default)
 {
  var user = await repository.FindByUsernameOrEmailAsync(request.UsernameOrEmail, ct);
  if (user is null || !user.IsActive || user.IsDeleted || user.LockedUntil > DateTimeOffset.UtcNow || user.PasswordHash is null || user.PasswordSalt is null || !passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
  {
   if (user is not null) await repository.UpdateLoginStateAsync(user.Id, false, ct);
   await repository.RecordLoginAsync(user?.Id, false, "Invalid credentials", ip, ua, ct); await unitOfWork.SaveChangesAsync(ct); throw new DomainException("Invalid credentials.");
  }
  await repository.UpdateLoginStateAsync(user.Id, true, ct); await repository.RecordLoginAsync(user.Id, true, null, ip, ua, ct);
  return await IssueTokensAsync(user, ip, ua, ct);
 }
 public async Task<TokenResponse> RefreshAsync(RefreshRequest request, string? ip, string? ua, CancellationToken ct = default)
 {
  var token = await repository.FindRefreshTokenAsync(tokenService.HashToken(request.RefreshToken), ct);
  if (token is null || token.RevokedAt is not null || token.UsedAt is not null || token.ExpiresAt <= DateTimeOffset.UtcNow) throw new DomainException("Invalid refresh token.");
  var user = await repository.FindByIdAsync(token.UserId, ct) ?? throw new DomainException("Invalid refresh token.");
  if (!user.IsActive || user.IsDeleted) throw new DomainException("User is inactive.");
  var roles = await repository.GetRolesAsync(user.Id, ct); var permissions = await repository.GetPermissionsAsync(user.Id, ct); var access = tokenService.CreateAccessToken(user.Id, user.Username, roles, permissions); var refresh = tokenService.CreateRefreshToken(); var refreshExpiry = DateTimeOffset.UtcNow.AddDays(7);
  await repository.RotateRefreshTokenAsync(token, access, tokenService.HashToken(access.Value), refresh, tokenService.HashToken(refresh), refreshExpiry, ip, ua, ct); await unitOfWork.SaveChangesAsync(ct);
  return new TokenResponse(access.Value, refresh, access.ExpiresAt, refreshExpiry);
 }
 public async Task LogoutAsync(Guid userId, string jwtId, CancellationToken ct = default) { if (!Guid.TryParse(jwtId, out var id)) throw new DomainException("Invalid token identifier."); await repository.RevokeAccessTokenAsync(userId, id, ct); await unitOfWork.SaveChangesAsync(ct); }
 public async Task<AuthenticatedUser?> GetCurrentUserAsync(Guid userId, CancellationToken ct = default) { var user = await repository.FindByIdAsync(userId, ct); return user is null ? null : new AuthenticatedUser(user.Id, user.Username, await repository.GetRolesAsync(user.Id, ct), await repository.GetPermissionsAsync(user.Id, ct)); }
 private async Task<TokenResponse> IssueTokensAsync(AuthUser user, string? ip, string? ua, CancellationToken ct) { var roles=await repository.GetRolesAsync(user.Id,ct); var permissions=await repository.GetPermissionsAsync(user.Id,ct); var access=tokenService.CreateAccessToken(user.Id,user.Username,roles,permissions); var refresh=tokenService.CreateRefreshToken(); var expiry=DateTimeOffset.UtcNow.AddDays(7); await repository.AddTokensAsync(user.Id,access,tokenService.HashToken(access.Value),refresh,tokenService.HashToken(refresh),expiry,ip,ua,ct); await unitOfWork.SaveChangesAsync(ct); return new(access.Value,refresh,access.ExpiresAt,expiry); }
}
