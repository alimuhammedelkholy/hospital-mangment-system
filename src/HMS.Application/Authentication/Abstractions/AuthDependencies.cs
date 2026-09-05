namespace HMS.Application.Authentication.Abstractions;
public interface IPasswordHasher { (string Hash, string Salt) Hash(string password); bool Verify(string password, string hash, string salt); }
public interface ITokenService { IssuedToken CreateAccessToken(Guid userId, string username, IReadOnlyCollection<string> roles, IReadOnlyCollection<string> permissions); string CreateRefreshToken(); byte[] HashToken(string token); }
public sealed record IssuedToken(string Value, Guid JwtId, DateTimeOffset ExpiresAt);
