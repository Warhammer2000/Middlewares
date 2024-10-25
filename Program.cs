using Microsoft.Extensions.Logging;
using Middlewares.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var logger = builder.Services.BuildServiceProvider().GetService<ILogger<Program>>();

Random random = new Random();

int selectedMiddlewareIndex = random.Next(1, 6); 

bool isErrorHandler = random.Next(0, 2) == 0;

logger?.LogInformation($"Какой Middleware выбрал рандомайзер: {selectedMiddlewareIndex}");
logger?.LogInformation($"Middleware Роль: {(isErrorHandler ? "Error Handler" : "Response Provider")}");

builder.Services.AddSingleton(new MiddlewareConfig
{
    SelectedMiddlewareIndex = selectedMiddlewareIndex
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseMiddleware<Middleware1>();


app.UseMiddleware<Middleware2>();
app.UseMiddleware<Middleware3>();
app.UseMiddleware<Middleware4>();
app.UseMiddleware<Middleware5>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


public class MiddlewareConfig
{
    public int SelectedMiddlewareIndex { get; set; }
}