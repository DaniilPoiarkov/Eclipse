using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.IdentityUsers;

public sealed class IdentityUserManager
{
    private readonly IIdentityUserRepository _identityUserRepository;

    public IdentityUserManager(IIdentityUserRepository identityUserRepository)
    {
        _identityUserRepository = identityUserRepository;
    }

    /// <summary>Creates the user asynchronous.</summary>
    /// <param name="name">The name.</param>
    /// <param name="surname">The surname.</param>
    /// <param name="userName">The userName.</param>
    /// <param name="chatId">The telegram chat identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Created user</returns>
    /// or
    /// username
    /// already exists</exception>
    public async Task<Result<IdentityUser>> CreateAsync(
        string name, string surname, string userName, long chatId, CancellationToken cancellationToken = default)
    {
        var alreadyExist = await _identityUserRepository.ContainsAsync(
            expression: u => u.ChatId == chatId,
            cancellationToken: cancellationToken);

        if (alreadyExist)
        {
            return UserDomainErrors.DuplicateData(nameof(chatId), chatId);
        }

        var identityUser = IdentityUser.Create(Guid.NewGuid(), name, surname, userName, chatId);

        return await _identityUserRepository.CreateAsync(identityUser, cancellationToken);
    }

    public Task<IdentityUser> UpdateAsync(IdentityUser identityUser, CancellationToken cancellationToken = default)
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

    public async Task<IdentityUser?> FindByUsernameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.UserName == userName, cancellationToken))
            .SingleOrDefault();
    }

    public async Task<IdentityUser?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await _identityUserRepository.GetByExpressionAsync(u => u.ChatId == chatId, cancellationToken))
            .SingleOrDefault();
    }
}
