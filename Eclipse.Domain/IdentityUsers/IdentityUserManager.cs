using Eclipse.Domain.IdentityUsers.Exceptions;
using Eclipse.Domain.Shared.IdentityUsers;

namespace Eclipse.Domain.IdentityUsers;

public class IdentityUserManager
{
    private readonly IIdentityUserRepository _identityUserRepository;

    public IdentityUserManager(IIdentityUserRepository identityUserRepository)
    {
        _identityUserRepository = identityUserRepository;
    }

    public async Task<IdentityUser?> CreateAsync(IdentityUserCreateDto createUserDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createUserDto, nameof(createUserDto));

        var withSameData = await _identityUserRepository.GetByExpressionAsync(
            expression: u => u.ChatId == createUserDto.ChatId || u.Username == createUserDto.Username,
            cancellationToken: cancellationToken);

        if (withSameData.Count != 0)
        {
            var withSameId = withSameData.FirstOrDefault(u => u.ChatId == createUserDto.ChatId);

            return withSameId is not null
                ? throw new DuplicateDataException(nameof(createUserDto.ChatId), createUserDto.ChatId)
                : throw new DuplicateDataException(nameof(createUserDto.Username), createUserDto.Username);
        }

        var identityUser = new IdentityUser(
                Guid.NewGuid(),
                createUserDto.Name,
                createUserDto.Surname,
                createUserDto.Username,
                createUserDto.ChatId,
                createUserDto.Culture,
                createUserDto.NotificationsEnabled);

        return await _identityUserRepository.CreateAsync(identityUser, cancellationToken);
    }

    public async Task<IdentityUser?> UpdateAsync(IdentityUser identityUser, CancellationToken cancellationToken = default)
    {
        return await _identityUserRepository.UpdateAsync(identityUser, cancellationToken);
    }

    public async Task<IReadOnlyList<IdentityUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _identityUserRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IdentityUser?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _identityUserRepository.FindAsync(id, cancellationToken);
    }

    public async Task<IdentityUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.Username == username, cancellationToken))
            .SingleOrDefault();
    }
    public async Task<IdentityUser?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.ChatId == chatId, cancellationToken))
            .SingleOrDefault();
    }
}
