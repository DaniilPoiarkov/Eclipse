using Eclipse.Common.Background;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using System.Net;

using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Notifications.FinishTodoItems;

internal sealed class FinishTodoItemsJob : JobWithArgs<FinishTodoItemsJobData>
{
    private readonly IStringLocalizer<FinishTodoItemsJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _userRepository;

    public FinishTodoItemsJob(
        IStringLocalizer<FinishTodoItemsJob> localizer,
        ICurrentCulture currentCulture,
        ITelegramBotClient client,
        IUserRepository userRepository,
        ILogger<FinishTodoItemsJob> logger) : base(logger)
    {
        _localizer = localizer;
        _currentCulture = currentCulture;
        _client = client;
        _userRepository = userRepository;
    }

    protected override async Task Execute(FinishTodoItemsJobData args, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            Logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var message = _localizer[
            $"Jobs:Evening:{(user.TodoItems.IsNullOrEmpty() ? "Empty" : "RemindMarkAsFinished")}",
            user.Name,
            user.TodoItems.Count
        ];

        try
        {
            await _client.SendMessage(user.ChatId, message, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e)
        {
            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
