using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers.Events;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Domain.IdentityUsers;

public sealed class IdentityUser : AggregateRoot
{
    private IdentityUser(Guid id, string name, string surname, string username, long chatId)
        : base(id)
    {
        Name = name;
        Surname = surname;
        Username = username;
        ChatId = chatId;
    }

    private IdentityUser() { }

    private readonly List<Reminder> _reminders = [];

    private readonly List<TodoItem> _todoItems = [];

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }

    public TimeSpan Gmt { get; private set; }

    public IReadOnlyCollection<Reminder> Reminders => _reminders.AsReadOnly();
    
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    internal static IdentityUser Create(Guid id, string name, string surname, string username, long chatId)
    {
        var user = new IdentityUser(id, name, surname, username, chatId);
        
        user.AddEvent(new NewUserJoinedDomainEvent(id, username, name, surname));

        return user;
    }

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
        var reminders = _reminders
            .Where(new ReminderNotifyAtSpecification(time))
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
        var now = DateTime.UtcNow.GetTime();

        var offset = currentUserTime - now;

        var day = TimeSpan.FromHours(24);

        if (offset < TimeSpan.FromHours(-12))
        {
            offset += day;
        }
        if (offset > TimeSpan.FromHours(12))
        {
            offset -= day;
        }

        Gmt = offset;
    }

    /// <summary>Adds the todo item.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Created TodoItem item</returns>
    public Result<TodoItem> AddTodoItem(string? text)
    {
        if (_todoItems.Count == TodoItemConstants.Limit)
        {
            return UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit);
        }

        var result = TodoItem.Create(Guid.NewGuid(), Id, text, DateTime.UtcNow.Add(Gmt));
        
        if (!result.IsSuccess)
        {
            return result.Error;
        }

        _todoItems.Add(result.Value);

        return result;
    }

    /// <summary>Finishes the item.</summary>
    /// <param name="todoItemId">The todo item identifier.</param>
    /// <returns>Removed <a cref="TodoItem"></a></returns>
    public Result<TodoItem> FinishItem(Guid todoItemId)
    {
        var item = _todoItems.FirstOrDefault(e => e.Id == todoItemId);

        if (item is null)
        {
            return UserDomainErrors.TodoItemNotFound();
        }

        _todoItems.Remove(item);

        var result = item.MarkAsFinished();

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return item;
    }
}
