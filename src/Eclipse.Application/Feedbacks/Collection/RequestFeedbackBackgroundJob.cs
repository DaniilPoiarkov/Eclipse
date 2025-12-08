using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Feedbacks.Collection;

internal sealed class RequestFeedbackBackgroundJob : IBackgroundJob<UserIdJobData>
{
    private readonly IUserRepository _userRepository;

    private readonly IFeedbackCollector _collector;

    private readonly ITelegramBotClient _client;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly ILogger<RequestFeedbackBackgroundJob> _logger;

    public RequestFeedbackBackgroundJob(
        IUserRepository userRepository,
        IFeedbackCollector collector,
        ITelegramBotClient client,
        IOptions<ApplicationOptions> options,
        ILogger<RequestFeedbackBackgroundJob> logger)
    {
        _userRepository = userRepository;
        _collector = collector;
        _client = client;
        _options = options;
        _logger = logger;
    }

    public async Task ExecuteAsync(UserIdJobData args, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot request feedback from user {Id}. {Reason}.", args.UserId, "User not found");

            await _client.SendMessage(_options.Value.Chat,
                $"❌ Feedback not requested from user: {args.UserId}\n\rUser not found.",
                cancellationToken: cancellationToken
            );

            return;
        }

        var displayName = user.GetReportingDisplayName();

        if (!user.IsEnabled)
        {
            _logger.LogWarning("Cannot request feedback from user {Id}. {Reason}.", args.UserId, "User is disabled");

            await _client.SendMessage(_options.Value.Chat,
                $"❌ Feedback not requested from user: {displayName}\n\rUser is disabled.",
                cancellationToken: cancellationToken
            );

            return;
        }

        try
        {
            await _collector.CollectAsync(args.UserId, cancellationToken);

            await _client.SendMessage(_options.Value.Chat,
                $"✅ Requested feedback from user: {displayName}",
                cancellationToken: cancellationToken
            );
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, "Failed to send collect feedback from user {UserId}. Disabling user.", args.UserId);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);

            await _client.SendMessage(_options.Value.Chat,
                $"❌ Failed to request feedback from user: {displayName}\n\rDisabling user.",
                cancellationToken: cancellationToken
            );
        }
    }
}
