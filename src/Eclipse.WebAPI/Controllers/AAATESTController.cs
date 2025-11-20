using Eclipse.Application;
using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Plots;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[Route("api/aaatest")]
[ApiController]
public class AAATESTController : ControllerBase
{
    private readonly IReportsService _reportsService;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly IPlotGenerator _plotGenerator;

    public AAATESTController(IReportsService reportsService, ITelegramBotClient telegramBotClient, IOptions<ApplicationOptions> options, IPlotGenerator plotGenerator)
    {
        _reportsService = reportsService;
        _telegramBotClient = telegramBotClient;
        _options = options;
        _plotGenerator = plotGenerator;
    }

    [HttpPost("send-test-message")]
    public async Task<IActionResult> SendTest(CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendMessage(_options.Value.Chat, "FINAL REVIEW from Eclipse WebAPI", cancellationToken: cancellationToken);

        foreach (var day in Enumerable.Range(31, 1))//new int[] { 26, 29, 31 }
        {
            var values = Enumerable.Range(1, day)
                .Select(i => (Value: Random.Shared.Next(1, 6), Date: new DateTime(2025, 12, i)))
                .ToList();

            using var stream = _plotGenerator.Create(new PlotOptions<DateTime, int>
            {
                Title = $"Test Plot for {day} days",
                Width = 600,
                Height = 400,
                Bottom = new AxisOptions<DateTime>
                {
                    Label = "Date",
                    Values = [.. values.Select(v => v.Date)],
                },
                Left = new AxisOptions<int>
                {
                    Label = "Values",
                    Min = 0.5d,
                    Max = 5.5d,
                    Values = [.. values.Select(v => v.Value)],
                },
            });

            await _telegramBotClient.SendPhoto(
                chatId: _options.Value.Chat,
                photo: InputFile.FromStream(stream, $"test-plot-{day}-days.png"),
                caption: $"Test message for day {day}",
                cancellationToken: cancellationToken
            );
        }

        return NoContent();
    }

    [HttpPost("send-report")]
    public async Task<IActionResult> SendReportAsync(CancellationToken cancellationToken)
    {
        using var stream = await _reportsService.GetMoodReportAsync(
            Guid.Parse("07b79dd3-8812-4938-89f4-5624c86e2bf1"),
            new MoodReportOptions
            {
                From = DateTime.UtcNow.FirstDayOfTheMonth().WithTime(0, 0),
                To = DateTime.UtcNow.WithTime(23, 59),
            },
            cancellationToken
        );

        await _telegramBotClient.SendPhoto(
            chatId: _options.Value.Chat,
            photo: InputFile.FromStream(stream, "mood-report.png"),
            caption: "Test Mood Report",
            cancellationToken: cancellationToken
        );

        return NoContent();
    }
}
