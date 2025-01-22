using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningJob : INotificationJob<GoodMorningJobData>
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<GoodMorningJob> _localizer;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<GoodMorningJob> _logger;

    public GoodMorningJob(
        ICurrentCulture currentCulture,
        IStringLocalizer<GoodMorningJob> localizer,
        ITelegramBotClient client,
        IUserRepository userRepository,
        ILogger<GoodMorningJob> logger)
    {
        _currentCulture = currentCulture;
        _localizer = localizer;
        _client = client;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(GoodMorningJobData args, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);

        await _client.SendMessage(
            chatId: user.ChatId,
            text: _localizer["Jobs:Morning:SendGoodMorning"],
            cancellationToken: cancellationToken
        );
    }
}
