using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eclipse.Application.MoodRecords.Report.Monthly;

internal sealed class MonthlyMoodReportJob : JobWithArgs<UserIdJobData>
{
    private readonly IUserRepository _userRepository;

    private readonly IMoodReportSender _sender;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<ApplicationOptions> _options;

    public MonthlyMoodReportJob(
        IUserRepository userRepository,
        IMoodReportSender sender,
        ITimeProvider timeProvider,
        ILogger<MonthlyMoodReportJob> logger,
        IOptions<ApplicationOptions> options) : base(logger)
    {
        _userRepository = userRepository;
        _sender = sender;
        _timeProvider = timeProvider;
        _options = options;
    }

    protected override async Task Execute(UserIdJobData args, CancellationToken cancellationToken)
    {
        // TODO: Temporary should not be available publicly
        var user = await _userRepository.FindByChatIdAsync(_options.Value.Chat, cancellationToken);

        if (args.UserId != user?.Id)
        {
            return;
        }

        var options = new SendMoodReportOptions(
            _timeProvider.Now.FirstDayOfTheMonth(),
            _timeProvider.Now.WithTime(23, 59),
            "Jobs:MoodReport:Monthly:Caption"
        );

        await _sender.Send(args.UserId, options, cancellationToken);
    }
}
