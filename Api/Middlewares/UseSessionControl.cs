using Application.Interfaces;
using Application.Interfaces.CacheInterfaces;
using Microsoft.AspNetCore.Authorization;

namespace Messanger.Middlewares;

public class UseSessionControl
{
    private readonly RequestDelegate _next;
    private readonly ISessionCache _sessionCache;

    public UseSessionControl(RequestDelegate next, ISessionCache sessionCache)
    {
        _next = next;
        _sessionCache = sessionCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sessionService = context.RequestServices.GetRequiredService<IUserSessionService>();

        var endpoint = context.GetEndpoint();

        var authorizeAttr = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
        if (authorizeAttr == null)
        {
            await _next(context);
            return;
        }
        if (!Guid.TryParse(context.Request.Headers["Session-Id"], out var sessionId))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("No session");
            return;
        }

        var session = await _sessionCache.HasSessionAsync(sessionId);

        if (session == false)
        {
            var basedSession = await sessionService.GetUserSessuionById(sessionId, ct: CancellationToken.None);
            if (basedSession is not null) return;
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid session");
            return;
        }

        context.Items["Session"] = sessionId;

        await _next(context);
    }
}