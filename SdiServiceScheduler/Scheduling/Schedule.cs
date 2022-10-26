namespace SdiServiceScheduler.Scheduling;

public class Schedule
{
    public string ServiceName { get; set; } = string.Empty;
    public List<DayOfWeek> OperationalDays { get; set; } = new();
    public bool IsEnabled { get; set; }
    public double Delay { get; set; }
    public bool RunAtStartUp { get; set; }
    public TimeSpan DelayTimeSpan { get; set; } = TimeSpan.Zero;
    public DateTime? StartTime { get; set; }
    public DateTime? StopTime { get; set; }
}