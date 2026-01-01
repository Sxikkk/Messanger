using Application.Contracts.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        try
        {
            var tokenResponse = await _authService.CreateUserAsync(request);
            return Ok(tokenResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            var tokenResponse = await _authService.LoginUser(request);
            return Ok(tokenResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequest request)
    {
        try
        {
            var token = await _authService.RefreshTokenAsync(request);
            if (token is null) return Unauthorized();
            
            return Ok(token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        await _authService.ConfirmEmail(token, email);
        return Ok("Email successfully confirmed! You can now log in.");
    }
    
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmationAsync([FromBody] ResendConfirmationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _authService.ResendConfirmationEmailAsync(request);
            return Ok(new { message = "Если email зарегистрирован и не подтверждён, письмо отправлено." });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "Произошла ошибка при отправке письма." });
        }
    }
}