using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Common.Results;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderService : IReminderService
{
    private readonly IdentityUserManager _userManager;

    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    public ReminderService(IdentityUserManager userManager, IMapper<IdentityUser, IdentityUserDto> mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<IdentityUserDto>> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.AddReminder(createReminderDto.Text, createReminderDto.NotifyAt);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> RemoveRemindersForTime(Guid userId, TimeOnly time, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

        user.RemoveRemindersForTime(time);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }
}
