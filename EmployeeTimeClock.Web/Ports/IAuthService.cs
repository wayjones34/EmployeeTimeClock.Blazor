using EmployeeTimeClock.Web.Domain.Entities;

namespace EmployeeTimeClock.Web.Ports;

public interface IAuthService
{
    Task<UserAccount?> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<UserAccount?> GetCurrentUserAsync();
    Task<IReadOnlyList<UserAccount>> GetAllUsersAsync();
}

