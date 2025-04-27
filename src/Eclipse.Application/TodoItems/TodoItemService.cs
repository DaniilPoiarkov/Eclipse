using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    public TodoItemService(IUserRepository userRepository, ITimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<UserDto>> CreateAsync(long chatId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await user.AddTodoItem(model.Text, _timeProvider.Now)
            .TapAsync(_ => _userRepository.UpdateAsync(user, cancellationToken))
            .BindAsync(_ => user.ToDto());
    }

    public async Task<Result<TodoItemDto>> CreateAsync(Guid userId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await user.AddTodoItem(model.Text, _timeProvider.Now)
            .TapAsync(_ => _userRepository.UpdateAsync(user, cancellationToken))
            .BindAsync(todoItem => todoItem.ToDto());
    }

    public async Task<Result<UserDto>> FinishAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await user.FinishItem(itemId, _timeProvider.Now)
            .TapAsync(_ => _userRepository.UpdateAsync(user, cancellationToken))
            .BindAsync(_ => user.ToDto());
    }

    public async Task<Result<List<TodoItemDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return user.TodoItems.Select(item => item.ToDto()).ToList();
    }

    public async Task<Result<TodoItemDto>> GetAsync(Guid userId, Guid todoItemId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var item = user.TodoItems.FirstOrDefault(item => item.Id == todoItemId);

        if (item is null)
        {
            return DefaultErrors.EntityNotFound<TodoItem>();
        }

        return item.ToDto();
    }
}
