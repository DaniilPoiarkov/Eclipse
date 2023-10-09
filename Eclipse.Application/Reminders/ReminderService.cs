using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Exceptions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.Reminders;

internal class ReminderService : IReminderService
{
    private readonly IIdentityUserCache _identityUserCache;

    private readonly IdentityUserManager _userManager;

    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    public ReminderService(IIdentityUserCache identityUserCache, IdentityUserManager userManager, IMapper<IdentityUser, IdentityUserDto> mapper)
    {
        _identityUserCache = identityUserCache;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IdentityUserDto> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        var reminder = new Reminder(Guid.NewGuid(), userId, createReminderDto.Text, createReminderDto.NotifyAt);

        user.AddReminder(reminder);

        await _userManager.UpdateAsync(user, cancellationToken);

        var dto = _mapper.Map(user);

        _identityUserCache.AddOrUpdate(dto);

        return dto;
    }
}
