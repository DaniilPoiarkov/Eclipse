using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using System.Net;

using Telegram.Bot.Exceptions;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed class CollectMoodRecordJob : JobWithArgs<CollectMoodRecordJobData>
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

    protected override async Task Execute(CollectMoodRecordJobData args, CancellationToken cancellationToken)
    {
        try
        {
            await _collector.CollectAsync(args.UserId, cancellationToken);
        }
        catch (ApiRequestException e)
        {
            Logger.LogError(e, "Failed to run {Job} job.", nameof(CollectMoodRecordJob));

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
