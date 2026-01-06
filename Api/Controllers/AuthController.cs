using System.Security.Claims;
using Application.Contracts.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromHeader] string deviceId, [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userAgentString = Request.Headers.UserAgent.ToString();
            var tokenResponse = await _authService.CreateUserAsync(request, userAgentString, deviceId, cancellationToken);
            if (tokenResponse is null) return Ok();
            Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"])),
                Path = "/"
            });
            return Ok(new { AccessToken = tokenResponse.AccessToken });

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, [FromHeader] string deviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userAgentString = Request.Headers.UserAgent.ToString();
            var tokenResponse = await _authService.LoginUser(request, userAgentString, deviceId, cancellationToken);
            Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"])),
                Path = "/"
            });
            return Ok(new { AccessToken = tokenResponse.AccessToken });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized();
            ArgumentNullException.ThrowIfNull(refreshToken);
            var token = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
            if (token is null) return Unauthorized();
            Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"])),
                Path = "/"
            });
            return Ok(new { AccessToken = token.AccessToken });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(string token, string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var confirm = await _authService.ConfirmEmail(token, email, cancellationToken);
            return Ok(confirm ? "Email confirmed" : "Email not confirmed");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("resend-confirmation")]
    [Authorize]
    public async Task<IActionResult> ResendConfirmationAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            ArgumentNullException.ThrowIfNull(userEmail);
            await _authService.ResendConfirmationEmailAsync(userEmail, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "Произошла ошибка при отправке письма" });
        }
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromHeader] string deviceId, CancellationToken ct = default)
    {
        try
        {
            var token = Request.Cookies.SingleOrDefault((k) => k.Key == "refreshToken").Value;

            await _authService.LogoutUserAsync(token, deviceId, ct);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}