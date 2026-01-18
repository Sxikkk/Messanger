using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Messanger.Middlewares;

public class UseSessionControl
{
    private readonly RequestDelegate _next;
    private readonly ICacheService _cacheService;

    public UseSessionControl(RequestDelegate next, ICacheService cacheService)
    {
        _next = next;
        _cacheService = cacheService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        var authorizeAttr = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
        if (authorizeAttr == null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Session-Id", out var sessionId))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("No session");
            return;
        }

        var session = await _cacheService.GetAsync<UserSession>($"session:{sessionId}");
        Console.WriteLine(sessionId);
        if (session == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid session");
            return;
        }

        context.Items["Session"] = session;

        await _next(context);
    }
}