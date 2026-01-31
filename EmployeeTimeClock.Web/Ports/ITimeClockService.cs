using EmployeeTimeClock.Web.Domain.Entities;

namespace EmployeeTimeClock.Web.Ports;

public interface ITimeClockService
{
    Task<bool> IsClockedInAsync(int userId);

    Task<TimeEntry?> GetOpenEntryAsync(int userId);

    Task<TimeEntry> ClockInAsync(int userId, DateTime now);

    Task<TimeEntry> ClockOutAsync(int userId, DateTime now);

    Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(int userId, DateTime from, DateTime to);
}
