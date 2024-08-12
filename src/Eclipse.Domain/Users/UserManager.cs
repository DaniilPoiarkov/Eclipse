using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Repositories;

using System.Linq.Expressions;

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
    public Task<Result<User>> CreateAsync(string name, string surname, string userName, long chatId, CancellationToken cancellationToken = default)
    {
        var request = new CreateUserRequest
        {
            Id = Guid.NewGuid(),
            Name = name,
            Surname = surname,
            UserName = userName,
            ChatId = chatId,
            NewRegistered = true,
        };

        return CreateAsync(request, cancellationToken);
    }

    public async Task<Result<User>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Name.IsNullOrEmpty())
        {
            return Error.Validation("Users.Create", "{0}IsRequired", nameof(request.Name));
        }

        if (request.Id.IsEmpty())
        {
            return Error.Validation("Users.Create", "{0}IsRequired", nameof(request.Id));
        }

        var alreadyExist = await _userRepository.ContainsAsync(
            expression: u => u.ChatId == request.ChatId || (!request.UserName.IsNullOrEmpty() && u.UserName == request.UserName),
            cancellationToken: cancellationToken);

        if (alreadyExist)
        {
            return UserDomainErrors.DuplicateData(nameof(request.ChatId), request.ChatId);
        }

        var user = User.Create(request.Id, request.Name, request.Surname, request.UserName, request.ChatId, request.NewRegistered);
        user.NotificationsEnabled = request.NotificationsEnabled;

        user.SetGmt(request.Gmt);

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

    public Task<IReadOnlyList<User>> GetByExpressionAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByExpressionAsync(expression, cancellationToken);
    }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (userName.IsNullOrEmpty())
        {
            return null;
        }

        return (await _userRepository.GetByExpressionAsync(u => u.UserName == userName, cancellationToken))
            .SingleOrDefault();
    }

    public async Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return (await _userRepository.GetByExpressionAsync(u => u.ChatId == chatId, cancellationToken))
            .SingleOrDefault();
    }
}
