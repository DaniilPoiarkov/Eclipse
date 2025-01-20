using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class RemindToFinishTodoItemsJob : INotificationJob<RemindToFinishTodoItemsJobData>
{
    private readonly IStringLocalizer<RemindToFinishTodoItemsJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<RemindToFinishTodoItemsJob> _logger;

    public RemindToFinishTodoItemsJob(
        IStringLocalizer<RemindToFinishTodoItemsJob> localizer,
        ICurrentCulture currentCulture,
        ITelegramBotClient client,
        IUserRepository userRepository,
        ILogger<RemindToFinishTodoItemsJob> logger)
    {
        _localizer = localizer;
        _currentCulture = currentCulture;
        _client = client;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(RemindToFinishTodoItemsJobData args, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var message = _localizer[
            $"Jobs:Evening:{(user.TodoItems.IsNullOrEmpty() ? "Empty" : "RemindMarkAsFinished")}",
            user.Name,
            user.TodoItems.Count
        ];

        await _client.SendMessage(user.ChatId, message, cancellationToken: cancellationToken);
    }
}
