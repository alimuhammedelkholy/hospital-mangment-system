using System.IdentityModel.Tokens.Jwt; using Microsoft.AspNetCore.Authentication; using System.Security.Claims; using HMS.Application.Authentication.Abstractions; using HMS.Application.Authentication.Contracts; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc;
namespace HMS.Api.Controllers;
[ApiController, Route("api/v1/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
 [HttpPost("register")] [ProducesResponseType(StatusCodes.Status201Created)] public async Task<IActionResult> Register(RegisterUserRequest request,CancellationToken ct){await auth.RegisterAsync(request,ct);return StatusCode(StatusCodes.Status201Created);}
 [HttpPost("login")] public Task<TokenResponse> Login(LoginRequest request,CancellationToken ct)=>auth.LoginAsync(request,HttpContext.Connection.RemoteIpAddress?.ToString(),Request.Headers.UserAgent.ToString(),ct);
 [HttpPost("refresh")] public Task<TokenResponse> Refresh(RefreshRequest request,CancellationToken ct)=>auth.RefreshAsync(request,HttpContext.Connection.RemoteIpAddress?.ToString(),Request.Headers.UserAgent.ToString(),ct);
 [Authorize, HttpPost("logout")] public async Task<IActionResult> Logout(CancellationToken ct){var id=Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);await auth.LogoutAsync(id,User.FindFirstValue(JwtRegisteredClaimNames.Jti)!,ct);return NoContent();}
 [Authorize, HttpGet("me")] public async Task<ActionResult<AuthenticatedUser>> Me(CancellationToken ct){var user=await auth.GetCurrentUserAsync(Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!),ct);return user is null?NotFound():Ok(user);}
}
