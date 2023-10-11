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
        var user = await FindById(id, cancellationToken);
        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> SetUserGmtTimeAsync(Guid userId, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        var user = await FindById(userId, cancellationToken);

        user.SetGmt(currentUserTime);

        await _userManager.UpdateAsync(user, cancellationToken);

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await FindById(id, cancellationToken);

        user.Name = updateDto.Name is null
            ? user.Name
            : updateDto.Name;

        user.Surname = updateDto.Surname is null
            ? user.Surname
            : updateDto.Surname;

        user.Username = updateDto.Username is null
            ? user.Username
            : updateDto.Username;

        if (!string.IsNullOrEmpty(updateDto.Culture))
        {
            user.Culture = updateDto.Culture;
        }

        if (updateDto.NotificationsEnabled.HasValue)
        {
            user.NotificationsEnabled = updateDto.NotificationsEnabled.Value;
        }

        var updated = await _userManager.UpdateAsync(user, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(updated);
    }

    private async Task<IdentityUser> FindById(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByIdAsync(userId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));
    }
}
