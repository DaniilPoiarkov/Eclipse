﻿using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly UserManager _userManager;

    private readonly IStringLocalizer<ReminderService> _localizer;

    public ReminderService(UserManager userManager, IStringLocalizer<ReminderService> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<ReminderDto>> CreateAsync(Guid userId, ReminderCreateDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User), _localizer);
        }

        var result = await CreateAsync(user, model, cancellationToken);

        return result.Value.ToDto();
    }

    public async Task<Result<UserDto>> CreateAsync(long chatId, ReminderCreateDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User), _localizer);
        }

        await CreateAsync(user, model, cancellationToken);

        return user.ToDto();
    }

    private async Task<Result<Reminder>> CreateAsync(User user, ReminderCreateDto model, CancellationToken cancellationToken)
    {
        var reminder = user.AddReminder(model.Text, model.NotifyAt);

        await _userManager.UpdateAsync(user, cancellationToken);

        return reminder;
    }

    public async Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User), _localizer);
        }

        var reminder = user.GetReminder(reminderId);

        if (reminder is null)
        {
            return DefaultErrors.EntityNotFound(typeof(Reminder), _localizer);
        }

        return reminder.ToDto();
    }

    public async Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User), _localizer);
        }

        return user.Reminders.Select(reminder => reminder.ToDto()).ToList();
    }

    public async Task<Result<UserDto>> RemoveForTimeAsync(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User), _localizer);
        }

        user.RemoveRemindersForTime(time);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
