namespace Eclipse.Application.Contracts.Reminders;

public interface IReminderService
{
    Task<IReadOnlyList<ReminderDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ReminderDto> CreateAsync(ReminderCreateDto createDto, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid reminderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReminderWithUserDto>> GetForSpecifiedTimeAsync(TimeOnly time, CancellationToken cancellationToken = default);
}
