using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Exceptions;

namespace Eclipse.Application.Notifications;

internal abstract class UserProcessingJob : JobWithArgs<UserIdJobData>
{
    protected readonly IUserRepository UserRepository;

    protected UserProcessingJob(IUserRepository userRepository, ILogger logger) : base(logger)
    {
        UserRepository = userRepository;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        var user = await UserRepository.FindAsync(args.UserId, cancellationToken);

        if (user is null)
        {
            Logger.LogError("User with id {UserId} not found", args.UserId);
            return;
        }

        try
        {
            await Execute(user, cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            Logger.LogError(ex, "Failed to run good morning job for user {UserId}. Disabling user.", args.UserId);

            user.SetIsEnabled(false);
            await UserRepository.UpdateAsync(user, cancellationToken);
        }
    }

    protected abstract Task Execute(User user, CancellationToken cancellationToken);
}
