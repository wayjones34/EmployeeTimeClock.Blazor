using EmployeeTimeClock.Web.Domain.Entities;

namespace EmployeeTimeClock.Web.State;

public class SessionState
{
    public UserAccount? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser is not null;

    public void SetUser(UserAccount user) => CurrentUser = user;

    public void Clear() => CurrentUser = null;

    public void Logout() => Clear();

    public bool IsAdmin => CurrentUser?.Role == "Admin";
}
