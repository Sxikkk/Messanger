using System.Security.Claims;
using Application.Contracts.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile/{username}")]
    public async Task<IActionResult> GetPublicUserProfileAsync(string username,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var shortUserInfo = await _userService.GetShortUserInfo(username, cancellationToken);
            return Ok(shortUserInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpGet("profile/my")]
    public async Task<IActionResult> GetUserProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            var userProfile = await _userService.GetUserProfileInfo(Guid.Parse(userId), cancellationToken);
            return Ok(userProfile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpPatch("profile/update")]
    public async Task<IActionResult> UpdateUserSettingAsync([FromBody] UpdateUserSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _userService.UpdateUserSettings(request, Guid.Parse(userId), cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }
}