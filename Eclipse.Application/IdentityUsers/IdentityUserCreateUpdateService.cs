using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal sealed class IdentityUserCreateUpdateService : IIdentityUserCreateUpdateService
{
    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    private readonly IdentityUserManager _userManager;

    public IdentityUserCreateUpdateService(IMapper<IdentityUser, IdentityUserDto> mapper, IdentityUserManager userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(createDto.Name, createDto.Surname, createDto.Username, createDto.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = result.Error;

            var ex = new DomainException(error!.Description);

            for (int i = 0; i < error.Args.Length; i++)
            {
                ex.WithData($"{{{i}}}", error.Args[i]);
            }

            throw ex;
        }

        return _mapper.Map(result.Value);
    }

    public async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

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
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

        return _mapper.Map(updated);
    }
}
