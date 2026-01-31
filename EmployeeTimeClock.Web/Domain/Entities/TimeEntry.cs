namespace EmployeeTimeClock.Web.Domain.Entities;

public class TimeEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime ClockIn { get; set; }

    public DateTime? ClockOut { get; set; }

    public int DurationMinutes =>
        ClockOut.HasValue
            ? (int)(ClockOut.Value - ClockIn).TotalMinutes
            : 0;
}
