using Eclipse.Domain.IdentityUsers.Exceptions;

namespace Eclipse.Domain.IdentityUsers;

public class IdentityUserManager
{
    private readonly IIdentityUserRepository _identityUserRepository;

    public IdentityUserManager(IIdentityUserRepository identityUserRepository)
    {
        _identityUserRepository = identityUserRepository;
    }

    public async Task<IdentityUser?> CreateAsync(
        string name, string surname, string username, long chatId, string culture, bool notificationsEnabled, CancellationToken cancellationToken = default)
    {
        var withSameData = await _identityUserRepository.GetByExpressionAsync(
            expression: u => u.ChatId == chatId || u.Username == username,
            cancellationToken: cancellationToken);

        if (withSameData.Count != 0)
        {
            var withSameId = withSameData.FirstOrDefault(u => u.ChatId == chatId);

            return withSameId is not null
                ? throw new DuplicateDataException(nameof(chatId), chatId)
                : throw new DuplicateDataException(nameof(username), username);
        }

        var identityUser = new IdentityUser(Guid.NewGuid(), name, surname, username, chatId, culture, notificationsEnabled);

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
