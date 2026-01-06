using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly IUserSessionService _userSessionService;

    public SessionsController(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserSessionsAsync(CancellationToken ct = default)
    {
        try
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            var sessions = await _userSessionService.GetUserSessionsByIdAsync(Guid.Parse(userId), ct);
            return Ok(sessions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpDelete("{deviceId}")]
    public async Task<IActionResult> TerminateSessionAsync(string deviceId, CancellationToken ct)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentToken = Request.Cookies["refreshToken"];
            await _userSessionService.TerminateSessionAsync(currentUserId, currentToken, deviceId, ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("terminate-all-others")]
    public async Task<IActionResult> TerminateAllOtherSessionsAsync(CancellationToken ct)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentToken = Request.Cookies["refreshToken"];

            if (currentToken != null) await _userSessionService.TerminateSessionsAsync(currentUserId, currentToken, ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}