using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Notifications.GoodMorning;

internal sealed class GoodMorningJob : JobWithArgs<UserIdJobData>
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<GoodMorningJob> _localizer;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _userRepository;

    public GoodMorningJob(
        ICurrentCulture currentCulture,
        IStringLocalizer<GoodMorningJob> localizer,
        ITelegramBotClient client,
        IUserRepository userRepository,
        ILogger<GoodMorningJob> logger) : base(logger)
    {
        _currentCulture = currentCulture;
        _localizer = localizer;
        _client = client;
        _userRepository = userRepository;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            Logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);

        try
        {
            await _client.SendMessage(
                chatId: user.ChatId,
                text: _localizer["Jobs:Morning:SendGoodMorning"],
                cancellationToken: cancellationToken
            );
        }
        catch (ApiRequestException ex)
        {
            Logger.LogError(ex, "Failed to run good morning job for user {UserId}. Disabling user.", args.UserId);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
