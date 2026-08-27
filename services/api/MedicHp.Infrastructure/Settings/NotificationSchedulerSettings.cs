namespace MedicHp.Infrastructure.Settings;

public class NotificationSchedulerSettings
{
    public int IntervalMinutes { get; set; } = 30;
    public int ReminderHoursBeforeAppointment { get; set; } = 24;
}
