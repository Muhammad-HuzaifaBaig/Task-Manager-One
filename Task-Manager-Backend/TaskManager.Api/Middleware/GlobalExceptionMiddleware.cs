using System.Net;
using Serilog;
using TaskManager.Model.Model.Common;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // 🔴 Log exception ONCE
            Log.Error(ex,"Unhandled exception occurred. RequestPath: {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ResponseMessage.Error("Internal server error.");
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
