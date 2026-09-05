namespace HMS.Application.Authentication.Contracts;
public sealed record RegisterUserRequest(string Username, string? Email, string? Phone, string Password);
public sealed record LoginRequest(string UsernameOrEmail, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record AuthenticatedUser(Guid UserId, string Username, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Permissions);
public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset AccessTokenExpiresAt, DateTimeOffset RefreshTokenExpiresAt);
