using System.Globalization;
using SdiServiceScheduler.Scheduling;

namespace SdiServiceScheduler.ServiceUtilties;

public static class ServiceSchedulerUtil
{
    public static TimeSpan? ParseTime(this string time)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("hr-HR");
        try
        {
            return TimeSpan.Parse(time);
        }
        catch (Exception)
        {
            /* Ignore the error */
        }

        return null;
    }

    public static bool IsEmpty(this string value)
    {
        return value.Trim().Length == 0;
    }
    
    public static bool IsNotEmpty(this string value)
    {
        return !IsEmpty(value);
    }
    
    public static TimeSpan GetDelay(this Schedule schedule, DateTime startTime)
    {
        DateTime now = DateTime.Now;
        TimeSpan ts = now.Subtract(startTime);
        return TimeSpan.FromTicks(schedule.DelayTimeSpan.Ticks - ts.Ticks);
    }

    public static bool IsServiceEnabled(this Schedule schedule, TimeSpan? delay)
    {
        if (schedule.ServiceName.IsEmpty()) return false;
        return delay is not null && schedule.IsEnabled;
    }

    public static bool IsReadyToExecute(this Schedule schedule)
    {
        return schedule.OperationalDays.Count == 0 
               || schedule.OperationalDays.Any(s => s == DateTime.Now.DayOfWeek);
    }

    public static bool IsTimeToRun(this Schedule schedule, bool runAtStartUp)
    {
        switch (runAtStartUp)
        {
            case true:
                return true;
            case false when schedule.RunAtStartUp:
                return true;
        }

        DateTime today = DateTime.Now;
        if (schedule.StartTime == default)
        {
            return true;
        }

        if (schedule.StartTime is null && schedule.StopTime is null)
        {
            return true;
        }
        
        if (schedule.StartTime is not null && schedule.StopTime is not null)
        {
            if (schedule.StartTime?.Hour == today.Hour && schedule.StartTime?.Minute == today.Minute && 
                today.Hour <= schedule.StopTime?.Hour && today.Minute <= schedule.StopTime?.Minute)
            {
                return true;
            }
        }
        else if (schedule.StartTime is not null && schedule.StopTime is null)
        {
            if (schedule.StartTime?.Hour == today.Hour && schedule.StartTime?.Minute == today.Minute)
            {
                return true;
            }
        }
        else if (schedule.StartTime is null && schedule.StopTime is not null)
        {
            if (today.Hour <= schedule.StopTime?.Hour && today.Minute <= schedule.StopTime?.Minute)
            {
                return true;
            }
        }
        
        return false;
    }
}