using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.IdentityUsers;

public class IdentityUser : AggregateRoot
{
    [JsonConstructor]
    internal IdentityUser(Guid id, string name, string surname, string username, long chatId, string culture, bool notificationsEnabled, List<Reminder>? reminders = null)
        : base(id)
    {
        Name = name;
        Surname = surname;
        Username = username;
        ChatId = chatId;
        Culture = culture;
        NotificationsEnabled = notificationsEnabled;
        Reminders = reminders ?? new List<Reminder>();
    }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; internal set; }

    public long ChatId { get; init; }

    public string Culture { get; private set; }

    public bool NotificationsEnabled { get; private set; }

    public List<Reminder> Reminders { get; set; }


    public void SetCulture(string culture) => Culture = culture;

    public void SwitchNotifications(bool notificationsEnabled) => NotificationsEnabled = notificationsEnabled;


    public void AddReminder(Reminder reminder) => Reminders.Add(reminder);

    public IReadOnlyList<Reminder> GetRemindersForTime(TimeOnly time) => Reminders.Where(r => r.NotifyAt == time).ToList();

    public IReadOnlyList<Reminder> GetReminders() => Reminders;

    public void RemoveReminders(IEnumerable<Reminder> reminders)
    {
        foreach (Reminder reminder in reminders)
        {
            Reminders.Remove(reminder);
        }
    }
}
