using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Exceptions;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.IdentityUsers;

#pragma warning disable CS8618
public class IdentityUser : AggregateRoot
{
    internal IdentityUser(Guid id, string name, string surname, string username, long chatId, string culture, bool notificationsEnabled)
        : base(id)
    {
        Name = name;
        Surname = surname;
        Username = username;
        ChatId = chatId;
        Culture = culture;
        NotificationsEnabled = notificationsEnabled;
        
        Gmt = default;

        _reminders = new List<Reminder>();
        _todoItems = new List<TodoItem>();
    }

    [JsonConstructor]
    private IdentityUser()
    {
        
    }

    [JsonProperty(nameof(Reminders))]
    private readonly List<Reminder> _reminders;

    [JsonProperty(nameof(TodoItems))]
    private readonly List<TodoItem> _todoItems;

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; set; }

    public long ChatId { get; init; }

    public string Culture { get; set; }

    public bool NotificationsEnabled { get; set; }

    [JsonProperty]
    public TimeSpan Gmt { get; private set; }

    [JsonIgnore]
    public IReadOnlyCollection<Reminder> Reminders => _reminders;
    
    [JsonIgnore]
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems;

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

    /// <summary>
    /// Calculates GMT using curernt user time and utc now
    /// </summary>
    /// <param name="currentUserTime"></param>
    public void SetGmt(TimeOnly currentUserTime)
    {
        var utc = DateTime.UtcNow;
        var now = new TimeOnly(utc.Hour, utc.Minute);

        var gmt = currentUserTime > now
            ? currentUserTime - now
            : (now - currentUserTime) * -1;

        var day = new TimeSpan(24, 0, 0);

        if (gmt < new TimeSpan(-12, 0, 0))
        {
            Gmt = gmt + day;
        }
        if (gmt > new TimeSpan(12, 0, 0))
        {
            Gmt = gmt - day;
        }
    }

    /// <summary>
    /// Creates new todo item to user
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <exception cref="TodoItemLimitException"></exception>
    public TodoItem AddTodoItem(string? text)
    {
        if (_todoItems.Count == TodoItemConstants.Limit)
        {
            throw new TodoItemLimitException(TodoItemConstants.Limit);
        }

        if (string.IsNullOrEmpty(text) || text.Length < TodoItemConstants.MinLength)
        {
            throw new TodoItemValidationException(TodoItemErrors.Messages.Empty);
        }

        if (text.Length > TodoItemConstants.MaxLength)
        {
            throw new TodoItemValidationException(TodoItemErrors.Messages.MaxLength);
        }

        var todoItem = new TodoItem(Guid.NewGuid(), Id, text, DateTime.UtcNow.Add(Gmt));
        _todoItems.Add(todoItem);
        return todoItem;
    }

    /// <summary>
    /// Removes item with given Id from user list of TodoItems and retuns it
    /// </summary>
    /// <param name="todoItemId"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public TodoItem FinishItem(Guid todoItemId)
    {
        var item = _todoItems.FirstOrDefault(i => i.Id == todoItemId)
            ?? throw new EntityNotFoundException(typeof(TodoItem));

        _todoItems.Remove(item);

        return item;
    }
}
#pragma warning restore CS8618
