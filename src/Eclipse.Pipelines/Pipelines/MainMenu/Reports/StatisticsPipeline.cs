using Eclipse.Application.Contracts.Statistics;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

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
        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!user.IsSuccess)
        {
            return Text("Error");
        }

        var statistics = await _userStatisticsService.GetByUserIdAsync(user.Value.Id, cancellationToken);

        return Text($"Todo items: {statistics.Value.TodoItemsFinished}; Reminders: {statistics.Value.RemindersReceived}");
    }
}
