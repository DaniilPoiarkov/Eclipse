using Eclipse.Common.Clock;
using Eclipse.Common.Extensions;
using Eclipse.Domain.ApiTokens;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Application.ApiTokens.Expiration;

internal sealed class NotifyExpiringApiTokensJob : IJob
{
    private readonly IApiTokenRepository _apiTokenRepository;

    private readonly IUserRepository _userRepository;

    private readonly ITelegramBotClient _botClient;

    private readonly IStringLocalizer<NotifyExpiringApiTokensJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<ApiTokenExpirationNotificationOptions> _options;

    private readonly ILogger<NotifyExpiringApiTokensJob> _logger;

    public NotifyExpiringApiTokensJob(
        IApiTokenRepository apiTokenRepository,
        IUserRepository userRepository,
        ITelegramBotClient botClient,
        IStringLocalizer<NotifyExpiringApiTokensJob> localizer,
        ICurrentCulture currentCulture,
        ITimeProvider timeProvider,
        IOptions<ApiTokenExpirationNotificationOptions> options,
        ILogger<NotifyExpiringApiTokensJob> logger)
    {
        _apiTokenRepository = apiTokenRepository;
        _userRepository = userRepository;
        _botClient = botClient;
        _localizer = localizer;
        _currentCulture = currentCulture;
        _timeProvider = timeProvider;
        _options = options;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var now = _timeProvider.Now;
        var to = _options.Value.Thresholds.Max();

        var tokens = await _apiTokenRepository.GetExpiringBetweenAsync(now, now.AddDays(to + 1), context.CancellationToken);

        var userIds = tokens.Select(t => t.UserId).Distinct();
        var users = await _userRepository.GetByExpressionAsync(user => userIds.Contains(user.Id), context.CancellationToken);

        var tokensGrouped = tokens.GroupBy(t => t.ExpiresAt.Date)
            .IntersectBy(_options.Value.Thresholds.Select(days => now.AddDays(days).Date), g => g.Key)
            .SelectMany(t => t)
            .GroupBy(t => t.UserId);

        await users.Join(tokensGrouped, u => u.Id, g => g.Key, (u, g) => (User: u, Tokens: g))
            .Select(tuple => SendAsync(tuple.User, tuple.Tokens, context.CancellationToken))
            .WhenAll();
    }

    private async Task SendAsync(User user, IEnumerable<ApiToken> tokens, CancellationToken cancellationToken)
    {
        try
        {
            using var _ = _currentCulture.UsingCulture(user.Culture);

            var text = tokens.OrderBy(t => t.ExpiresAt)
                .Select(t => $"🔑 {t.Name}: {t.MaskedValue} - {t.ExpiresAt:yyyy-MM-dd}")
                .Join(Environment.NewLine);

            var message = _localizer["Jobs:ApiTokens:ExpiringSoon{Tokens}", text];

            await _botClient.SendMessage(user.ChatId, message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send API token expiration notification to user {UserId}", user.Id);
        }
    }
}
