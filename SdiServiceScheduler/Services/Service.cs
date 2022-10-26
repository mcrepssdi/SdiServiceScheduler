namespace SdiServiceScheduler.Services;

public class Service
{
    public string Delay { get; set; } = string.Empty;
    public string DelayIn { get; set; } = string.Empty;
    public string OperationalDays { get; set; } = string.Empty;
    public string Enabled { get; set; } = string.Empty;
    public string RunAtStartUp { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string StopTime { get; set; } = string.Empty;
}