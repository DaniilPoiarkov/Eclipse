using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Feedbacks.Collection;

internal sealed class CollectFeedbackJob : JobWithArgs<UserIdJobData>
{
    private readonly IUserRepository _userRepository;

    private readonly IFeedbackCollector _collector;

    public CollectFeedbackJob(ILogger<CollectFeedbackJob> logger, IUserRepository userRepository, IFeedbackCollector collector) : base(logger)
    {
        _userRepository = userRepository;
        _collector = collector;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        try
        {
            await _collector.CollectAsync(args.UserId, cancellationToken);
        }
        catch (ApiRequestException e)
        {
            Logger.LogError(e, "Failed to send collect mood record job for user {UserId}. Disabling user.", args.UserId);

            var user = await _userRepository.FindAsync(args.UserId, cancellationToken);

            if (user is null)
            {
                return;
            }

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
