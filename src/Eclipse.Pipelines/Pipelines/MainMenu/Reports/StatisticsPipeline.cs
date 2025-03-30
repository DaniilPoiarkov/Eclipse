using Eclipse.Application.Contracts.Statistics;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reports;

[Route("Menu:Reports:Statistics", "/reports_statistics")]
internal sealed class StatisticsPipeline : ReportsPipelineBase
{
    private readonly IUserService _userService;

    private readonly IUserStatisticsService _userStatisticsService;

    public StatisticsPipeline(IUserService userService, IUserStatisticsService userStatisticsService)
    {
        _userService = userService;
        _userStatisticsService = userStatisticsService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendStatisticsAsync);
    }

    private async Task<IResult> SendStatisticsAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Text(Localizer.LocalizeError(result.Error));
        }

        var statistics = await _userStatisticsService.GetByUserIdAsync(result.Value.Id, cancellationToken);

        return Text(Localizer["Pipelines:Statistics:Report{0}{1}", statistics.TodoItemsFinished, statistics.RemindersReceived]);
    }
}
