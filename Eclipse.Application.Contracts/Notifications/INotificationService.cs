namespace Eclipse.Application.Contracts.Notifications;

public interface INotificationService
{
    Task AddJob();

    Task DeleteJob(string group);

    Task AddTestJob(long id, TimeZoneInfo timeZone);
}
