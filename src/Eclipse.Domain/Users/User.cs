using Eclipse.Common.Results;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users.Events;

namespace Eclipse.Domain.Users;

public sealed class User : AggregateRoot
{
    private User(Guid id, string name, string surname, string userName, long chatId, DateTime createdAt, bool isEnabled)
        : base(id)
    {
        Name = name;
        Surname = surname;
        UserName = userName;
        ChatId = chatId;
        CreatedAt = createdAt;
        IsEnabled = isEnabled;
    }

    private User() { }

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; set; } = string.Empty;

    public string SignInCode { get; private set; } = string.Empty;
    public DateTime? SignInCodeExpiresAt { get; private set; }

    public bool NotificationsEnabled { get; set; }

    public bool IsEnabled { get; private set; } = true;

    public TimeSpan Gmt { get; private set; }

    public DateTime CreatedAt { get; private set; }


    private readonly List<Reminder> _reminders = [];

    private readonly List<TodoItem> _todoItems = [];

    public IReadOnlyCollection<Reminder> Reminders => _reminders.AsReadOnly();
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    internal static User Create(Guid id, string name, string surname, string userName, long chatId, DateTime createdAt, bool isEnabled, bool newRegistered)
    {
        var user = new User(id, name, surname, userName, chatId, createdAt, isEnabled);

        if (newRegistered)
        {
            user.AddEvent(new NewUserJoinedDomainEvent(id, userName, name, surname));
        }

        return user;
    }

    /// <summary>Adds the reminder.</summary>
    /// <param name="text">The text.</param>
    /// <param name="notifyAt">The notify at.</param>
    /// <returns>Created Reminder</returns>
    public Reminder AddReminder(string text, TimeOnly notifyAt)
    {
        return AddReminder(Guid.CreateVersion7(), text, notifyAt);
    }

    public Reminder AddReminder(Guid id, string text, TimeOnly notifyAt)
    {
        var reminder = new Reminder(id, Id, text, notifyAt.Add(Gmt * -1));
        _reminders.Add(reminder);

        AddEvent(new ReminderAddedDomainEvent(id, Id, Gmt, reminder.NotifyAt, text, Culture, ChatId));

        return reminder;
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

        AddEvent(new GmtChangedDomainEvent(Id, Gmt));
    }

    public void SetGmt(TimeSpan gmt)
    {
        Gmt = gmt;
        AddEvent(new GmtChangedDomainEvent(Id, Gmt));
    }

    /// <summary>Adds the todo item.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Created TodoItem item</returns>
    public Result<TodoItem> AddTodoItem(string? text, DateTime createdAt)
    {
        return AddTodoItem(Guid.CreateVersion7(), text, createdAt, false, default);
    }

    public Result<TodoItem> AddTodoItem(Guid id, string? text, DateTime createdAt, bool isFinished, DateTime? finishedAt)
    {
        if (_todoItems.Count == TodoItemConstants.Limit)
        {
            return UserDomainErrors.TodoItemsLimit(TodoItemConstants.Limit);
        }

        if (_todoItems.Exists(item => item.Id == id))
        {
            return UserDomainErrors.DuplicateTodoItem(id);
        }

        var result = TodoItem.Create(id, Id, text, createdAt, isFinished, finishedAt);

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

        AddEvent(new TodoItemFinishedDomainEvent(Id));

        return item;
    }

    /// <summary>
    /// Sets new sign in code
    /// </summary>
    public void SetSignInCode(DateTime utcNow)
    {
        if (SignInCodeExpiresAt > utcNow)
        {
            return;
        }

        SignInCode = UserConsts.GenerateSignInCode();
        SignInCodeExpiresAt = utcNow.Add(UserConsts.SignInCodeExpiration);
    }

    public void TriggerTestEvent()
    {
        AddEvent(new TestDomainEvent(ChatId));
    }

    public bool IsValidSignInCode(DateTime utcNow, string signInCode)
    {
        return !SignInCode.IsNullOrEmpty()
            && SignInCode == signInCode
            && utcNow < SignInCodeExpiresAt;
    }

    public MoodRecord CreateMoodRecord(MoodState state, DateTime createdAt)
    {
        return new MoodRecord(Guid.CreateVersion7(), Id, state, createdAt.WithTime(0, 0));
    }

    public Reminder? ReceiveReminder(Guid reminderId)
    {
        var reminder = _reminders.FirstOrDefault(r => r.Id == reminderId);

        if (reminder is not null)
        {
            _reminders.Remove(reminder);
            AddEvent(new RemindersReceivedDomainEvent(Id));
        }

        return reminder;
    }

    public void SetCreatedAtIfNull(DateTime createdAt)
    {
        if (CreatedAt == default)
        {
            CreatedAt = createdAt;
        }
    }

    public void SetIsEnabled(bool isEnabled)
    {
        if (IsEnabled == isEnabled)
        {
            return;
        }

        IsEnabled = isEnabled;

        if (IsEnabled == true)
        {
            AddEvent(new UserEnabledDomainEvent(Id));
        }
        else
        {
            AddEvent(new UserDisabledDomainEvent(Id));
        }
    }

    public override string ToString()
    {
        return $"{Name}, {UserName} {base.ToString()}";
    }
}
