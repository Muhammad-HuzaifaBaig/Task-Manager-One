using Serilog.Context;
using System.Security.Claims;

public class LogUserContextMiddleware
{
    private readonly RequestDelegate _next;

    public LogUserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = context.User?.Identity?.Name;

        using (LogContext.PushProperty("UserId", userId ?? "Anonymous"))
        using (LogContext.PushProperty("Username", username ?? "Anonymous"))
        {
            await _next(context);
        }
    }
}
