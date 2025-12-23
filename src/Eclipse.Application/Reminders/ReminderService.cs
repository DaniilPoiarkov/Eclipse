using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Reminders.Processors;
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

    public async Task<Result<List<ReminderDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return user.Reminders.Select(reminder => reminder.ToDto()).ToList();
    }

    public Task<Result<ReminderDto>> GetAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        return Process(userId, new GetReminderByIdProcessor(reminderId), cancellationToken);
    }

    public Task<Result<ReminderDto>> DeleteAsync(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        return Process(userId, new RemoveReminderProcessor(reminderId), cancellationToken);
    }

    public Task<Result<ReminderDto>> RescheduleAsync(Guid userId, Guid reminderId, RescheduleReminderOptions options, CancellationToken cancellationToken = default)
    {
        return Process(userId, new RescheduleReminderProcessor(reminderId, options), cancellationToken);
    }

    public Task<Result<ReminderDto>> GetByRelatedItem(Guid userId, Guid relatedItemId, CancellationToken cancellationToken = default)
    {
        return Process(userId, new GetReminderByRelatedItemIdProcessor(relatedItemId), cancellationToken);
    }

    public Task<Result<ReminderDto>> Receive(Guid userId, Guid reminderId, CancellationToken cancellationToken = default)
    {
        return Process(userId, new ReceiveReminderProcessor(reminderId), cancellationToken);
    }

    private async Task<Result<ReminderDto>> Process(Guid userId, IReminderProcessor processor, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var reminder = processor.Process(user);

        if (reminder is null)
        {
            return DefaultErrors.EntityNotFound<Reminder>();
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return reminder.ToDto();
    }
}
