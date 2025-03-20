using Eclipse.Common.Background;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using System.Net;

using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Notifications.GoodMorning;

internal sealed class GoodMorningJob : JobWithArgs<GoodMorningJobData>
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

    protected override async Task Execute(GoodMorningJobData args, CancellationToken cancellationToken)
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
        catch (ApiRequestException e)
        {
            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
