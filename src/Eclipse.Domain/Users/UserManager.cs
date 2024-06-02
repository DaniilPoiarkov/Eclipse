﻿using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.Users;

public sealed class UserManager
{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
    public async Task<Result<User>> CreateAsync(string name, string surname, string userName, long chatId, CancellationToken cancellationToken = default)
    {
        var alreadyExist = await _userRepository.ContainsAsync(
            expression: u => u.ChatId == chatId,
            cancellationToken: cancellationToken);

        if (alreadyExist)
        {
            return UserDomainErrors.DuplicateData(nameof(chatId), chatId);
        }

        var user = User.Create(Guid.NewGuid(), name, surname, userName, chatId);

        return await _userRepository.CreateAsync(user, cancellationToken);
    }

    public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        return _userRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _userRepository.GetAllAsync(cancellationToken);
    }

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _userRepository.FindAsync(id, cancellationToken);
    }

    public async Task<User?> FindByUsernameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return (await _userRepository.GetByExpressionAsync(u => u.UserName == userName, cancellationToken))
            .SingleOrDefault();
    }

    public async Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await _userRepository.GetByExpressionAsync(u => u.ChatId == chatId, cancellationToken))
            .SingleOrDefault();
    }
}