﻿using Eclipse.Common.Results;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users.Events;

using Newtonsoft.Json;

namespace Eclipse.Domain.Users;

public sealed class User : AggregateRoot
{
    private User(Guid id, string name, string surname, string userName, long chatId)
        : base(id)
    {
        Name = name;
        Surname = surname;
        UserName = userName;
        ChatId = chatId;
    }

    private User() { }

    [JsonProperty(nameof(Reminders))]
    private readonly List<Reminder> _reminders = [];

    [JsonProperty(nameof(TodoItems))]
    private readonly List<TodoItem> _todoItems = [];

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; init; }

    public string Culture { get; set; } = string.Empty;

    public string SignInCode { get; private set; } = string.Empty;
    public DateTime? SignInCodeExpiresAt { get; private set; }

    public bool NotificationsEnabled { get; set; }


    public TimeSpan Gmt { get; private set; }

    [JsonIgnore]
    public IReadOnlyCollection<Reminder> Reminders => _reminders.AsReadOnly();

    [JsonIgnore]
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    internal static User Create(Guid id, string name, string surname, string userName, long chatId, bool newRegistered)
    {
        var user = new User(id, name, surname, userName, chatId);

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
        return AddReminder(Guid.NewGuid(), text, notifyAt);
    }

    public Reminder AddReminder(Guid id, string text, TimeOnly notifyAt)
    {
        var reminder = new Reminder(id, Id, text, notifyAt);
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

    public void SetGmt(TimeSpan gmt)
    {
        Gmt = gmt;
    }

    /// <summary>Adds the todo item.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Created TodoItem item</returns>
    public Result<TodoItem> AddTodoItem(string? text, DateTime createdAt)
    {
        return AddTodoItem(Guid.NewGuid(), text, createdAt, false, default);
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

    public TodoItem? GetTodoItem(Guid todoItemId)
    {
        return _todoItems.FirstOrDefault(item => item.Id == todoItemId);
    }

    public Reminder? GetReminder(Guid reminderId)
    {
        return _reminders.FirstOrDefault(reminder => reminder.Id == reminderId);
    }

    public MoodRecord CreateMoodRecord(MoodState state, DateTime createdAt)
    {
        return new MoodRecord(Guid.NewGuid(), Id, state, createdAt);
    }

    public override string ToString()
    {
        return $"{Name}, {UserName} {base.ToString()}";
    }
}
