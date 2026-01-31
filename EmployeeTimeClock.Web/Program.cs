using EmployeeTimeClock.Web.Components;
using EmployeeTimeClock.Web.Ports;
using EmployeeTimeClock.Web.Infrastructure.Fake;
using EmployeeTimeClock.Web.State;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<IAuthService, FakeAuthService>();
builder.Services.AddSingleton<ITimeClockService, FakeTimeClockService>();
builder.Services.AddScoped<SessionState>();
builder.Services.AddScoped<SessionTimeoutService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Login endpoint - handles form POST
app.MapPost("/api/login", async (HttpContext context, IAuthService auth) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var user = await auth.LoginAsync(username, password);
    if (user is null)
    {
        context.Response.Redirect("/login?error=1");
        return;
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Role, user.Role),
        new("DisplayName", user.DisplayName)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    context.Response.Redirect("/dashboard");
});

// Logout endpoint
app.MapGet("/api/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/login");
});

// Map Razor Components
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
