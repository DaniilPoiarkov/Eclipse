using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly IUserRepository _userRepository;

    public ReminderService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<ReminderDto>> CreateAsync(Guid userId, ReminderCreateDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await CreateAsync(user, model, cancellationToken)
            .BindAsync(reminder => reminder.ToDto());
    }

    public async Task<Result<UserDto>> CreateAsync(long chatId, ReminderCreateDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await CreateAsync(user, model, cancellationToken)
            .BindAsync(_ => user.ToDto());
    }

    private async Task<Result<Reminder>> CreateAsync(User user, ReminderCreateDto model, CancellationToken cancellationToken)
    {
        var reminder = user.AddReminder(model.RelatedItemId, model.Text, model.NotifyAt);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return reminder;
    }

    public async Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var reminder = user.Reminders.FirstOrDefault(r => r.Id == reminderId);

        if (reminder is null)
        {
            return DefaultErrors.EntityNotFound<Reminder>();
        }

        return reminder.ToDto();
    }

    public async Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return user.Reminders.Select(reminder => reminder.ToDto()).ToList();
    }

    // TODO: rename as ReceiveAsync with options whether to remove reminder or not.
    public async Task<Result> DeleteAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var reminder = user.ReceiveReminder(reminderId, true);

        if (reminder is null)
        {
            return DefaultErrors.EntityNotFound<Reminder>();
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ReminderDto>> RescheduleAsync(Guid userId, Guid reminderId, DateTime notifyAt, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var reminder = user.RescheduleReminder(reminderId, notifyAt.GetTime());

        if (!reminder.IsSuccess)
        {
            return reminder.Error;
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return reminder.Value.ToDto();
    }
}
