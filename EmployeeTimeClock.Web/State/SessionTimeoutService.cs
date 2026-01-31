using Microsoft.JSInterop;

namespace EmployeeTimeClock.Web.State;

/// <summary>
/// Manages session inactivity detection and automatic logout.
/// Monitors user activity and logs out users after a configurable timeout period.
/// </summary>
public class SessionTimeoutService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<SessionTimeoutService>? _objRef;
    private IJSObjectReference? _module;
    private bool _isInitialized;

    public SessionState SessionState { get; }
    public event Func<Task>? OnTimeout;
    public event Func<Task>? OnWarning;

    /// <summary>Timeout in minutes. Default is 30 minutes.</summary>
    public int TimeoutMinutes { get; set; } = 30;

    /// <summary>Warning time in minutes before timeout. Default is 5 minutes.</summary>
    public int WarningMinutes { get; set; } = 5;

    public SessionTimeoutService(IJSRuntime jsRuntime, SessionState sessionState)
    {
        _jsRuntime = jsRuntime;
        SessionState = sessionState;
    }

    /// <summary>
    /// Initialize the session timeout monitoring.
    /// Call this from OnAfterRenderAsync in a component.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        try
        {
            _objRef = DotNetObjectReference.Create(this);
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./Components/sessionTimeout.js");

            await _module.InvokeVoidAsync("initSessionTimeout", 
                _objRef, 
                TimeoutMinutes * 60,      // Convert to seconds
                WarningMinutes * 60,
                TimeoutMinutes * 60 - WarningMinutes * 60);

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            // Session timeout not critical if JS fails
            Console.WriteLine($"Session timeout initialization failed: {ex.Message}");
        }
    }

    /// <summary>Called from JavaScript when warning threshold is reached.</summary>
    [JSInvokable("OnSessionWarning")]
    public async Task HandleWarning()
    {
        if (OnWarning != null)
        {
            await OnWarning.Invoke();
        }
    }

    /// <summary>Called from JavaScript when timeout is reached.</summary>
    [JSInvokable("OnSessionTimeout")]
    public async Task HandleTimeout()
    {
        SessionState.Logout();
        if (OnTimeout != null)
        {
            await OnTimeout.Invoke();
        }
    }

    /// <summary>Reset the inactivity timer when user interacts with the page.</summary>
    public async Task ResetTimeoutAsync()
    {
        if (_module == null) return;

        try
        {
            await _module.InvokeVoidAsync("resetSessionTimeout");
        }
        catch { /* Ignore errors in reset */ }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("cleanup");
                await _module.DisposeAsync();
            }
            catch { /* Ignore cleanup errors */ }
        }

        _objRef?.Dispose();
    }
}
