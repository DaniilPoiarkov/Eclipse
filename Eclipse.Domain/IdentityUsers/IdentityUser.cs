using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Exceptions;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.IdentityUsers;

public class IdentityUser : AggregateRoot
{
    [JsonConstructor]
    internal IdentityUser(
        Guid id,
        string name,
        string surname,
        string username,
        long chatId,
        string culture,
        bool notificationsEnabled,
        List<Reminder>? reminders = null,
        TimeSpan gmt = default,
        List<TodoItem>? todoItems = null)
        : base(id)
    {
        Name = name;
        Surname = surname;
        Username = username;
        ChatId = chatId;
        Culture = culture;
        NotificationsEnabled = notificationsEnabled;
        Gmt = gmt;

        _reminders = reminders ?? new List<Reminder>();
        _todoItems = todoItems ?? new List<TodoItem>();
    }

    private readonly List<Reminder> _reminders;

    private readonly List<TodoItem> _todoItems;

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; internal set; }

    public long ChatId { get; init; }

    public string Culture { get; set; }

    public bool NotificationsEnabled { get; set; }

    public TimeSpan Gmt { get; private set; }

    public IReadOnlyCollection<Reminder> Reminders => _reminders;

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

        Gmt = currentUserTime > now
            ? currentUserTime - now
            : (now - currentUserTime) * -1;
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
