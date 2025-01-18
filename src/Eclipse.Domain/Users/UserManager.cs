using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.Users;

public sealed class UserManager
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    public UserManager(IUserRepository repository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
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
            Id = Guid.CreateVersion7(),
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

        var alreadyExist = await _repository.ContainsAsync(
            expression: u => u.ChatId == request.ChatId || (!request.UserName.IsNullOrEmpty() && u.UserName == request.UserName),
            cancellationToken: cancellationToken
        );

        if (alreadyExist)
        {
            return UserDomainErrors.DuplicateData(nameof(request.ChatId), request.ChatId);
        }

        var user = User.Create(request.Id, request.Name, request.Surname, request.UserName, request.ChatId, _timeProvider.Now, request.NewRegistered);
        user.NotificationsEnabled = request.NotificationsEnabled;

        user.SetGmt(request.Gmt);

        return await _repository.CreateAsync(user, cancellationToken);
    }
}
