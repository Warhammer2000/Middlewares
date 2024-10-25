using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class Middleware1
{
    public static readonly Guid Id = Guid.NewGuid();
    private readonly RequestDelegate _next;
    private readonly MiddlewareConfig _config;
    private readonly ILogger<Middleware1> _logger;

    public Middleware1(RequestDelegate next, MiddlewareConfig config, ILogger<Middleware1> logger)
    {
        _next = next;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var middlewareGuids = context.Items["MiddlewareGuids"] as List<Guid> ?? new List<Guid>();
        middlewareGuids.Add(Id);
        context.Items["MiddlewareGuids"] = middlewareGuids;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            LogException(ex, middlewareGuids);
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An error occurred in Middleware " + Id);
            }
            else
            {
                _logger.LogWarning("Response has already started, cannot set StatusCode.");
            }
        }
    }

    private void LogException(Exception ex, List<Guid> middlewareGuids)
    {
        var stackTrace = new StringBuilder();
        stackTrace.AppendLine("Stack Trace:");
        foreach (var guid in middlewareGuids)
        {
            stackTrace.AppendLine($"Middleware GUID: {guid}");
        }
        stackTrace.AppendLine($"Exception Type: {ex.GetType().Name}");
        stackTrace.AppendLine($"Message: {ex.Message}");

        _logger.LogError(stackTrace.ToString());
    }
}
