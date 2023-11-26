using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Exceptions;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.IdentityUsers;

public sealed class IdentityUser : AggregateRoot
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private IdentityUser()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
    public IReadOnlyCollection<Reminder> Reminders => _reminders.AsReadOnly();
    
    [JsonIgnore]
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

    /// <summary>Adds the reminder.</summary>
    /// <param name="text">The text.</param>
    /// <param name="notifyAt">The notify at.</param>
    /// <returns>Created Reminder</returns>
    public Reminder AddReminder(string text, TimeOnly notifyAt)
    {
        var reminder = new Reminder(Guid.NewGuid(), Id, text, notifyAt);
        _reminders.Add(reminder);

        return reminder;
    }

    /// <summary>Removes the reminders which matches provided time.</summary>
    /// <param name="time">The time.</param>
    /// <returns>Removed Reminders</returns>
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

    /// <summary>Sets the GMT by given input with user local time.</summary>
    /// <param name="currentUserTime">The current user time.</param>
    public void SetGmt(TimeOnly currentUserTime)
    {
        var utc = DateTime.UtcNow;
        var now = new TimeOnly(utc.Hour, utc.Minute);

        Gmt = currentUserTime > now
            ? currentUserTime - now
            : (now - currentUserTime) * -1;

        var day = new TimeSpan(24, 0, 0);

        if (Gmt < new TimeSpan(-12, 0, 0))
        {
            Gmt += day;
        }
        if (Gmt > new TimeSpan(12, 0, 0))
        {
            Gmt -= day;
        }
    }

    /// <summary>Adds the todo item.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Created TodoItem item</returns>
    /// <exception cref="TodoItemLimitException">New item exceeds maximum limit</exception>
    /// <exception cref="TodoItemValidationException">Text is empty or exceeds maximum length</exception>
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

    /// <summary>Finishes the item.</summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <returns>Removed <a cref="TodoItem"></a></returns>
    /// <exception cref="EntityNotFoundException">Item with given id not found</exception>
    public TodoItem FinishItem(Guid todoItemId)
    {
        var item = _todoItems.GetById(todoItemId);

        _todoItems.Remove(item);

        item.MarkAsFinished();

        return item;
    }
}
