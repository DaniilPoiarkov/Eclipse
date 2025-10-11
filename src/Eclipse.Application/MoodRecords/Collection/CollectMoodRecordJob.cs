using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Exceptions;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed class CollectMoodRecordJob : JobWithArgs<UserIdJobData>
{
    private readonly IMoodRecordCollector _collector;

    private readonly IUserRepository _userRepository;

    public CollectMoodRecordJob(
        IMoodRecordCollector collector,
        IUserRepository userRepository,
        ILogger<CollectMoodRecordJob> logger) : base(logger)
    {
        _collector = collector;
        _userRepository = userRepository;
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
