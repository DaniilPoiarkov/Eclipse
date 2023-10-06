using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Reminders;

namespace Eclipse.Application.IdentityUsers;

internal class IdentityUserService : IIdentityUserService
{
    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    private readonly IdentityUserManager _userManager;

    public IdentityUserService(IMapper<IdentityUser, IdentityUserDto> mapper, IdentityUserManager userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var identity = await _userManager.CreateAsync(
            createDto.Name, createDto.Surname, createDto.Username, createDto.ChatId, createDto.Culture, createDto.NotificationsEnabled,
            cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(identity);
    }

    public async Task<IdentityUserDto> CreateReminderAsync(Guid userId, ReminderCreateDto createReminderDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        var reminder = new Reminder(Guid.NewGuid(), userId, createReminderDto.Text, createReminderDto.NotifyAt);

        user.AddReminder(reminder);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.GetAllAsync(cancellationToken);
        
        return users
            .Select(_mapper.Map)
            .ToList();
    }

    public async Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        user.Name = updateDto.Name is null
            ? user.Name
            : updateDto.Name;

        user.Surname = updateDto.Surname is null
            ? user.Surname
            : updateDto.Surname;

        if (!string.IsNullOrEmpty(updateDto.Culture))
        {
            user.SetCulture(updateDto.Culture);
        }

        if (updateDto.NotificationsEnabled.HasValue)
        {
            user.SwitchNotifications(updateDto.NotificationsEnabled.Value);
        }

        var updated = await _userManager.UpdateAsync(user, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(updated);
    }
}
