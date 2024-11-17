using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Application.Users;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.TodoItems;

internal sealed class TodoItemService : ITodoItemService
{
    private readonly UserManager _userManager;

    private readonly ITimeProvider _timeProvider;

    private readonly IStringLocalizer<TodoItemService> _localizer;

    public TodoItemService(
        UserManager userManager,
        ITimeProvider timeProvider,
        IStringLocalizer<TodoItemService> localizer)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
        _localizer = localizer;
    }

    public async Task<Result<UserDto>> CreateAsync(long chatId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>(_localizer);
        }

        var result = await CreateAsync(user, model, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToLocalized(_localizer);
        }

        return user.ToDto();
    }

    public async Task<Result<TodoItemDto>> CreateAsync(Guid userId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>(_localizer);
        }

        var result = await CreateAsync(user, model, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToLocalized(_localizer);
        }

        return result.Value.ToDto();
    }

    private async Task<Result<TodoItem>> CreateAsync(User user, CreateTodoItemDto model, CancellationToken cancellationToken)
    {
        var result = user.AddTodoItem(model.Text, _timeProvider.Now);

        if (result.IsSuccess)
        {
            await _userManager.UpdateAsync(user, cancellationToken);
        }

        return result;
    }

    public async Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>(_localizer);
        }

        var result = user.FinishItem(itemId);

        if (!result.IsSuccess)
        {
            return result.Error.ToLocalized(_localizer);
        }

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<List<TodoItemDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>(_localizer);
        }

        return user.TodoItems.Select(item => item.ToDto()).ToList();
    }

    public async Task<Result<TodoItemDto>> GetAsync(Guid userId, Guid todoItemId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>(_localizer);
        }

        var item = user.GetTodoItem(todoItemId);

        if (item is null)
        {
            return DefaultErrors.EntityNotFound<TodoItem>(_localizer);
        }

        return item.ToDto();
    }
}
