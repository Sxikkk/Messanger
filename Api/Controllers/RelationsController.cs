using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[ApiController]
[Authorize]
[Route("api/users/relations")]
public class RelationsController : ControllerBase
{
    private readonly IUserRelationService _relationService;
    
    public RelationsController(IUserRelationService relationService)
    {
        _relationService = relationService;
    }

    [HttpPost("friends/request/{targetUsername}")]
    public async Task<IActionResult> SendFriendRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.SendFriendRequestAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpPost("friends/accept/{targetUsername}")]
    public async Task<IActionResult> AcceptFriendRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.AcceptFriendRequestAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpPost("friends/reject/{targetUsername}")]
    public async Task<IActionResult> RejectFriendRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.RejectFriendRequestAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpDelete("friends/{targetUsername}")]
    public async Task<IActionResult> RemoveFriendRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.RemoveFriendAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpGet("friends/{targetUsername}/block")]
    public async Task<IActionResult> BlockUserRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.BlockUserAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpDelete("friends/{targetUsername}/block")]
    public async Task<IActionResult> UnblockUserRequestAsync(string targetUsername, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            await _relationService.UnblockUserAsync(targetUsername, Guid.Parse(userId), ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(500);
        }
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriendsAsync(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            var users = await _relationService.GetFriendsAsync(Guid.Parse(userId), ct);
            return Ok(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("friends/requests")]
    public async Task<IActionResult> GetIncomingFriendsRequestsAsync(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ArgumentNullException.ThrowIfNull(userId);
            var requests = await _relationService.GetIncomingFriendRequestsAsync(Guid.Parse(userId), ct);
            return Ok(requests);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}