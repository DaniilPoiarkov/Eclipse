using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Reminders;

internal class ReminderService : IReminderService
{
    private readonly ReminderManager _reminderManager;

    private readonly IReminderRepository _reminderRepository;

    private readonly IMapper<Reminder, ReminderDto> _mapper;

    private readonly IIdentityUserService _identityUserService;

    public ReminderService(ReminderManager reminderManager, IReminderRepository reminderRepository, IMapper<Reminder, ReminderDto> mapper, IIdentityUserService identityUserService)
    {
        _reminderManager = reminderManager;
        _reminderRepository = reminderRepository;
        _mapper = mapper;
        _identityUserService = identityUserService;
    }

    public async Task<ReminderDto> CreateAsync(ReminderCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderManager.CreateAsync(createDto.UserId, createDto.Text, createDto.NotifyAt, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(Reminder));

        return _mapper.Map(reminder);
    }

    public Task DeleteAsync(Guid reminderId, CancellationToken cancellationToken = default)
    {
        return _reminderManager.DeleteAsync(reminderId, cancellationToken);
    }

    public async Task<IReadOnlyList<ReminderWithUserDto>> GetForSpecifiedTimeAsync(TimeOnly time, CancellationToken cancellationToken = default)
    {
        var reminders = await _reminderRepository.GetByExpressionAsync(n => n.NotifyAt == time, cancellationToken);
        var users = await _identityUserService.GetAllAsync(cancellationToken);

        return reminders
            .Select(_mapper.Map)
            .Join(users, r => r.UserId, u => u.Id, (reminder, user) => new ReminderWithUserDto
            {
                NotifyAt = reminder.NotifyAt,
                Text = reminder.Text,
                User = user
            })
            .ToList();
    }

    public async Task<IReadOnlyList<ReminderDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var reminder = await _reminderManager.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(Reminder));

        return reminder
            .Select(_mapper.Map)
            .ToList();
    }
}
