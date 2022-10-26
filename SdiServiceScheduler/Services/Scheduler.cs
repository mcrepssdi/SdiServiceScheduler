using Microsoft.Extensions.Configuration;
using SdiServiceScheduler.Scheduling;
using SdiServiceScheduler.ServiceUtilties;

namespace SdiServiceScheduler.Services;

public class Scheduler : ISchedule
{
    public List<Schedule> Schedules { get; set; } = new();
    
    public Scheduler() { }

    /// <summary>
    /// Creates a Series of Service Schedules from configuration file.
    /// </summary>
    /// <param name="config">Sepcific IConfiguration</param>
    public Scheduler(IConfiguration config)
    {
        IConfigurationSection serviceSection = config.GetSection("Services");
        foreach (IConfigurationSection? child in serviceSection.GetChildren())
        {
            Service? service = child.Get<Service>() ?? null;
            if (service is null) continue;

            Schedule s1 = BuildSchedule(service, child.Key);
            Schedules.Add(s1);
        }
    }
    
    /// <summary>
    /// Creates a Schedule for a Single Service
    /// </summary>
    /// <param name="serviceName">Desired Service Name</param>
    /// <param name="service">Service Information</param>
    public Scheduler(string serviceName, Service service)
    {
        Schedule s = BuildSchedule(service);
        s.ServiceName = serviceName;
        Schedules.Add(s);
    }
    
    public Schedule BuildSchedule(Service service, string serviceName = "")
    {
        Schedule schedule = new();
        
        TimeSpan? ts = service.StartTime.ParseTime();
        if (ts is not null)
        {
            DateTime dt = DateTime.Now;
            schedule.StartTime = dt.Date + ts.Value;
        }
        
        ts = service.StopTime.ParseTime();
        if (ts is not null)
        {
            DateTime dt = DateTime.Now;
            schedule.StopTime = dt.Date + ts.Value;
        }
        
        bool success = bool.TryParse(service.Enabled, out bool tf);
        schedule.IsEnabled = success switch
        {
            true => tf,
            _ => false
        };
        
        success = bool.TryParse(service.RunAtStartUp, out tf);
        schedule.RunAtStartUp = success switch
        {
            true => tf,
            _ => false
        };
        
        success = double.TryParse(service.Delay, out double dNum);
        schedule.Delay = success switch
        {
            true => dNum,
            _ => 0
        };
        
        _delayTimeSpan = service.DelayIn switch
        {
            "Milliseconds" => TimeSpan.FromMilliseconds(schedule.Delay),
            "Seconds" => TimeSpan.FromSeconds(schedule.Delay),
            "Minutes" => TimeSpan.FromMinutes(schedule.Delay),
            "Hours" => TimeSpan.FromHours(schedule.Delay),
            "Days" => TimeSpan.FromDays(schedule.Delay),
            _ => null
        };
        if (_delayTimeSpan is not null) schedule.DelayTimeSpan = _delayTimeSpan.Value;
        
        if (service.OperationalDays.IsNotEmpty())
        {
            schedule.OperationalDays = (from s in service.OperationalDays.Split(",").ToList() select (DayOfWeek)Enum.Parse(typeof(DayOfWeek), s)).ToList();
        }

        return schedule;
    }

}