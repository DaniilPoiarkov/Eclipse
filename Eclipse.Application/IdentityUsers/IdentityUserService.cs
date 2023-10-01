using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.IdentityUsers;

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

    public async Task<IdentityUserDto?> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var identity = await _userManager.CreateAsync(createDto, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(identity);
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAll()
    {
        var users = await _userManager.GetAllAsync();
        
        return users
            .Select(_mapper.Map)
            .ToList();
    }

    public async Task<IdentityUserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto?> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        user.Name = string.IsNullOrEmpty(updateDto.Name) ? user.Name : updateDto.Name;
        user.Surname = string.IsNullOrEmpty(updateDto.Surname) ? user.Surname : updateDto.Surname;

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
