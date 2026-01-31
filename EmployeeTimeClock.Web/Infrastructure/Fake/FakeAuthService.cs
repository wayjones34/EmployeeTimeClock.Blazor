using EmployeeTimeClock.Web.Domain.Entities;
using EmployeeTimeClock.Web.Ports;

namespace EmployeeTimeClock.Web.Infrastructure.Fake;

public class FakeAuthService : IAuthService
{
    // Fake "database" of users
    private readonly List<UserAccount> _users =
    [
        new UserAccount { Id = 1, Username = "demo",  DisplayName = "Demo Employee", Role = "Employee" },
        new UserAccount { Id = 2, Username = "admin", DisplayName = "Admin User",    Role = "Admin" }
    ];

    private UserAccount? _currentUser;

    public Task<UserAccount?> LoginAsync(string username, string password)
    {
        // Fake password rule: username == password (demo/demo, admin/admin)
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );

        if (user is null) return Task.FromResult<UserAccount?>(null);

        var ok = password == username;
        _currentUser = ok ? user : null;

        return Task.FromResult(_currentUser);
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        return Task.CompletedTask;
    }

    public Task<UserAccount?> GetCurrentUserAsync()
        => Task.FromResult(_currentUser);

    public Task<IReadOnlyList<UserAccount>> GetAllUsersAsync()
        => Task.FromResult((IReadOnlyList<UserAccount>)_users.AsReadOnly());
}
