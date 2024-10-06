using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Common.Clock;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports;

internal sealed class MoodReportPipeline : ReportsPipelineBase
{
    private readonly IUserService _userService;

    private readonly IReportsService _reportsService;

    private readonly ITimeProvider _timeProvider;

    public MoodReportPipeline(
        IUserService userService,
        IReportsService reportsService,
        ITimeProvider timeProvider)
    {
        _userService = userService;
        _reportsService = reportsService;
        _timeProvider = timeProvider;
    }

    protected override void Initialize()
    {
        RegisterStage(SendMoodReportAsync);
    }

    private async Task<IResult> SendMoodReportAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Text(Localizer.LocalizeError(result.Error));
        }

        var user = result.Value;

        var options = new MoodReportOptions
        {
            From = _timeProvider.Now.PreviousDayOfWeek(DayOfWeek.Sunday),
            To = _timeProvider.Now
        };

        using var stream = await _reportsService.GetMoodReportAsync(user.Id, options, cancellationToken);

        throw new NotImplementedException();
    }
}
