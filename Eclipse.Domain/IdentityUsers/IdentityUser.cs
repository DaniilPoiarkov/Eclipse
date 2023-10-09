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

        _reminders = reminders ?? new List<Reminder>();
    }

    private readonly List<Reminder> _reminders;

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; internal set; }

    public long ChatId { get; init; }

    public string Culture { get; set; }

    public bool NotificationsEnabled { get; set; }

    public IReadOnlyCollection<Reminder> Reminders => _reminders;

    /// <summary>
    /// Creates reminder for user and returns it
    /// </summary>
    /// <param name="text"></param>
    /// <param name="notifyAt"></param>
    /// <returns cref="Reminder"></returns>
    public Reminder AddReminder(string text, TimeOnly notifyAt)
    {
        var reminder = new Reminder(Guid.NewGuid(), Id, text, notifyAt);
        _reminders.Add(reminder);

        return reminder;
    }

    /// <summary>
    /// Removes reminders and returns those which match specified time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IReadOnlyList<Reminder> RemoveRemindersForTime(TimeOnly time)
    {
        var specification = new ReminderNotifyAtSpecification(time);

        var reminders = _reminders
            .Where(specification)
            .ToList();

        foreach (var reminder in reminders)
        {
            _reminders.Remove(reminder);
        }

        return reminders;
    }
}
