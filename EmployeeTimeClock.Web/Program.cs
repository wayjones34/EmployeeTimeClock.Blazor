using EmployeeTimeClock.Web.Components;
using EmployeeTimeClock.Web.Ports;
using EmployeeTimeClock.Web.Infrastructure.Fake;
using EmployeeTimeClock.Web.State;



var builder = WebApplication.CreateBuilder(args);

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
app.UseAntiforgery();

// ? THIS is what makes /login and /dashboard work
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
