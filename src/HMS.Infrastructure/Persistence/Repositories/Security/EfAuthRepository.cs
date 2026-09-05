using HMS.Application.Authentication.Abstractions;
using HMS.Infrastructure.Persistence.Context;
using HMS.Infrastructure.Persistence.Entities.Security;
using Microsoft.EntityFrameworkCore;
namespace HMS.Infrastructure.Persistence.Repositories.Security;
internal sealed class EfAuthRepository(HmsDbContext db) : IAuthRepository
{
 public Task<bool> UsernameExistsAsync(string v,CancellationToken ct)=>db.AppUsers.AnyAsync(x=>x.Username==v,ct); public Task<bool> EmailExistsAsync(string v,CancellationToken ct)=>db.AppUsers.AnyAsync(x=>x.Email==v,ct);
 public Task AddUserAsync(AuthUser u,CancellationToken ct){db.AppUsers.Add(new AppUser{UserId=u.Id,Username=u.Username,Email=u.Email,Phone=u.Phone,PasswordHash=u.PasswordHash,PasswordSalt=u.PasswordSalt,IsActive=true,IsDeleted=false,CreatedAt=DateTime.UtcNow});return Task.CompletedTask;}
 public async Task<AuthUser?> FindByUsernameOrEmailAsync(string v,CancellationToken ct) { var user=await db.AppUsers.SingleOrDefaultAsync(x=>x.Username==v||x.Email==v,ct); return user is null?null:ToModel(user); }
 public async Task<AuthUser?> FindByIdAsync(Guid id,CancellationToken ct) { var user=await db.AppUsers.SingleOrDefaultAsync(x=>x.UserId==id,ct); return user is null?null:ToModel(user); }
 public async Task<IReadOnlyCollection<string>> GetRolesAsync(Guid id,CancellationToken ct)=>await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.RoleId where ur.UserId==id&&!r.IsDeleted select r.RoleCode).ToListAsync(ct);
 public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid id,CancellationToken ct)=>await (from ur in db.UserRoles join rp in db.RolePermissions on ur.RoleId equals rp.RoleId join p in db.Permissions on rp.PermissionId equals p.PermissionId where ur.UserId==id&&!p.IsDeleted select p.PermissionCode).Distinct().ToListAsync(ct);
 public Task RecordLoginAsync(Guid? id,bool ok,string? reason,string? ip,string? ua,CancellationToken ct){db.LoginHistories.Add(new LoginHistory{UserId=id,LoginAt=DateTime.UtcNow,IsSuccessful=ok,FailureReason=reason,IPAddress=ip,UserAgent=ua,CreatedAt=DateTime.UtcNow});return Task.CompletedTask;}
 public async Task UpdateLoginStateAsync(Guid id,bool ok,CancellationToken ct){var u=await db.AppUsers.SingleAsync(x=>x.UserId==id,ct);u.FailedLoginCount=ok?0:u.FailedLoginCount+1;u.LastLoginAt=ok?DateTime.UtcNow:u.LastLoginAt;}
 public Task AddTokensAsync(Guid uid,IssuedToken a,byte[] ah,string refresh,byte[] rh,DateTimeOffset re,string? ip,string? ua,CancellationToken ct){AddTokenRows(uid,a,ah,refresh,rh,re,ip,ua);return Task.CompletedTask;}
 public async Task<RefreshTokenRecord?> FindRefreshTokenAsync(byte[] h,CancellationToken ct)=>await db.RefreshTokens.Where(x=>x.TokenHash==h).Select(x=>new RefreshTokenRecord(x.RefreshTokenId,x.UserId,x.JwtId,x.TokenHash,x.ExpiresAt,x.RevokedAt,x.UsedAt)).SingleOrDefaultAsync(ct);
 public Task RotateRefreshTokenAsync(RefreshTokenRecord t,IssuedToken a,byte[] ah,string r,byte[] rh,DateTimeOffset re,string? ip,string? ua,CancellationToken ct){var old=db.RefreshTokens.Local.SingleOrDefault(x=>x.RefreshTokenId==t.Id)??new RefreshToken{RefreshTokenId=t.Id}; if(db.Entry(old).State==EntityState.Detached) db.Attach(old); old.UsedAt=DateTime.UtcNow;old.RevokedAt=DateTime.UtcNow; AddTokenRows(t.UserId,a,ah,r,rh,re,ip,ua,old); return Task.CompletedTask;}
 public async Task RevokeAccessTokenAsync(Guid uid,Guid j,CancellationToken ct){var t=await db.AccessTokens.SingleOrDefaultAsync(x=>x.UserId==uid&&x.JwtId==j&&x.RevokedAt==null,ct);if(t is not null)t.RevokedAt=DateTime.UtcNow;}
 private void AddTokenRows(Guid uid,IssuedToken a,byte[] ah,string r,byte[] rh,DateTimeOffset re,string? ip,string? ua,RefreshToken? old=null){var rid=Guid.NewGuid();if(old is not null)old.ReplacedByTokenId=rid;db.AccessTokens.Add(new AccessToken{AccessTokenId=Guid.NewGuid(),UserId=uid,JwtId=a.JwtId,TokenHash=ah,IssuedAt=DateTime.UtcNow,ExpiresAt=a.ExpiresAt.UtcDateTime,DeviceIP=ip,UserAgent=ua});db.RefreshTokens.Add(new RefreshToken{RefreshTokenId=rid,UserId=uid,JwtId=a.JwtId,TokenHash=rh,IssuedAt=DateTime.UtcNow,ExpiresAt=re.UtcDateTime});}
 private static AuthUser ToModel(AppUser x)=>new(x.UserId,x.Username,x.Email,x.PasswordHash,x.PasswordSalt,x.IsActive,x.IsDeleted,x.LockedUntil,x.FailedLoginCount);
}
