namespace Middlewares.MiddleWare;

public class Middleware4
{
    public static readonly Guid Id = Guid.NewGuid();
    private readonly RequestDelegate _next;
    private readonly MiddlewareConfig _config;
    private readonly ILogger<Middleware4> _logger;
    public Middleware4(RequestDelegate next, MiddlewareConfig config, ILogger<Middleware4> logger)
    {
        _next = next;
        _config = config;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var middlewareGuids = context.Items["MiddlewareGuids"] as List<Guid> ?? new List<Guid>();
        middlewareGuids.Add(Id);
        context.Items["MiddlewareGuids"] = middlewareGuids;

        int currentMiddlewareIndex = 2;

        if (currentMiddlewareIndex == _config.SelectedMiddlewareIndex)
        {
            _logger.LogInformation($"Middleware {Id} (Index: {currentMiddlewareIndex}).");
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Response from Middleware {Id}");
            }
        }
        else
        {
            _logger.LogInformation($"Middleware {Id} (Index: {currentMiddlewareIndex}).");
            await _next(context);
        }
    }
}