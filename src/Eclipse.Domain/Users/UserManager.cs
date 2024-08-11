using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.Importing;
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

        if (request.Id == Guid.Empty)
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

    public Task ImportAsync(ImportUserDto model, CancellationToken cancellationToken = default)
    {
        var user = User.Create(model.Id, model.Name, model.Surname, model.UserName, model.ChatId, false);

        user.NotificationsEnabled = model.NotificationsEnabled;
        user.Culture = model.Culture;

        user.SetGmt(TimeSpan.Parse(model.Gmt));

        return _userRepository.CreateAsync(user, cancellationToken);
    }

    public async Task<Result> ImportTodoItemsAsync(Guid userId, IEnumerable<ImportTodoItemDto> todoItems, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.ImportTodoItems(todoItems);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ImportRemindersAsync(Guid userId, IEnumerable<ImportReminderDto> reminders, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.ImportReminders(reminders);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
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
