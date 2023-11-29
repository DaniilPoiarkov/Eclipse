namespace Eclipse.Domain.IdentityUsers;

public class IdentityUserManager
{
    private readonly IIdentityUserRepository _identityUserRepository;

    public IdentityUserManager(IIdentityUserRepository identityUserRepository)
    {
        _identityUserRepository = identityUserRepository;
    }

    /// <summary>Creates the user asynchronous.</summary>
    /// <param name="name">The name.</param>
    /// <param name="surname">The surname.</param>
    /// <param name="username">The username.</param>
    /// <param name="chatId">The telegram chat identifier.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="notificationsEnabled">if set to <c>true</c> [notifications enabled].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Created user</returns>
    /// <exception cref="DuplicateDataException">User with given chatId
    /// or
    /// username
    /// already exists</exception>
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

    public Task<IdentityUser?> UpdateAsync(IdentityUser identityUser, CancellationToken cancellationToken = default)
    {
        return _identityUserRepository.UpdateAsync(identityUser, cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _identityUserRepository.GetAllAsync(cancellationToken);
    }

    public Task<IdentityUser?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _identityUserRepository.FindAsync(id, cancellationToken);
    }

    public async Task<IdentityUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.Username == username, cancellationToken))
            .SingleOrDefault();
    }

    // TODO: Refactor tests so this method no longer need to be virtual and class can be marked as sealed
    public virtual async Task<IdentityUser?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.ChatId == chatId, cancellationToken))
            .SingleOrDefault();
    }
}
