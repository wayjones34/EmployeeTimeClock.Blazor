using EmployeeTimeClock.Web.Domain.Entities;
using EmployeeTimeClock.Web.Ports;

namespace EmployeeTimeClock.Web.Infrastructure.Fake;

public class FakeTimeClockService : ITimeClockService
{
    private readonly List<TimeEntry> _entries = new();
    private int _nextId = 1;

    public Task<bool> IsClockedInAsync(int userId)
        => Task.FromResult(_entries.Any(e => e.UserId == userId && e.ClockOut == null));

    public Task<TimeEntry?> GetOpenEntryAsync(int userId)
    {
        var open = _entries
            .Where(e => e.UserId == userId && e.ClockOut == null)
            .OrderByDescending(e => e.ClockIn)
            .FirstOrDefault();

        return Task.FromResult(open);
    }

    public Task<TimeEntry> ClockInAsync(int userId, DateTime now)
    {
        if (_entries.Any(e => e.UserId == userId && e.ClockOut == null))
            throw new InvalidOperationException("Already clocked in.");

        var entry = new TimeEntry
        {
            Id = _nextId++,
            UserId = userId,
            ClockIn = now
        };

        _entries.Add(entry);
        return Task.FromResult(entry);
    }

    public Task<TimeEntry> ClockOutAsync(int userId, DateTime now)
    {
        var open = _entries
            .FirstOrDefault(e => e.UserId == userId && e.ClockOut == null);

        if (open is null)
            throw new InvalidOperationException("Not clocked in.");

        open.ClockOut = now;
        return Task.FromResult(open);
    }

    public Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(int userId, DateTime from, DateTime to)
    {
        var items = _entries
            .Where(e => e.UserId == userId && e.ClockIn >= from && e.ClockIn <= to)
            .OrderByDescending(e => e.ClockIn)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyList<TimeEntry>)items);
    }
}
