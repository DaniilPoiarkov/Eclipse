using Eclipse.Common.Clock;
using Eclipse.Common.Plots;
using Eclipse.Common.Telegram;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AAATestController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<AAATestController> _localizer;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<TelegramOptions> _options;

    private readonly ICurrentCulture _currentCulture;

    private readonly IPlotGenerator _plotGenerator;

    public AAATestController(
        IUserRepository userRepository,
        IMoodRecordRepository moodRecordRepository,
        ITelegramBotClient client,
        IStringLocalizer<AAATestController> localizer,
        ITimeProvider timeProvider,
        IOptions<TelegramOptions> options,
        ICurrentCulture currentCulture,
        IPlotGenerator plotGenerator)
    {
        _userRepository = userRepository;
        _moodRecordRepository = moodRecordRepository;
        _client = client;
        _localizer = localizer;
        _timeProvider = timeProvider;
        _options = options;
        _currentCulture = currentCulture;
        _plotGenerator = plotGenerator;
    }

    [HttpGet]
    public async Task<IActionResult> Test(CancellationToken cancellationToken)
    {
        var date = new DateTime(2024, 10, 2);
        var time = date.GetTime();

        var users = (await _userRepository.GetByExpressionAsync(u =>
                u.NotificationsEnabled && u.ChatId == _options.Value.Chat, cancellationToken))
            .ToList();

        var after = date.AddDays(-4).PreviousDayOfWeek(DayOfWeek.Sunday);

        var moodRecords = await _moodRecordRepository.GetByExpressionAsync(
            mr => users.Select(u => u.Id).Contains(mr.UserId) && mr.CreatedAt >= after,
            cancellationToken
        );

        var grouping = moodRecords.GroupBy(mr => mr.UserId);

        foreach (var group in grouping)
        {
            if (group.IsNullOrEmpty())
            {
                continue;
            }

            var user = users.FirstOrDefault(u => u.Id == group.Key);

            if (user is null)
            {
                continue;
            }

            using var _ = _currentCulture.UsingCulture(user.Culture);

            var message = $"{_localizer["Jobs:SendMoodReport:Greeting"]}" +
                $"{Environment.NewLine}" +
                $"{_localizer["Jobs:SendMoodReport:Conclusion"]}";
            
            var (days, states) = GetPlotData(group);

            var options = new PlotOptions<DateTime, int>
            {
                Title = $"Mood report {after:dd.MM}-{date:dd.MM}",
                YAxisTitle = "Score",
                Width = 550,
                Height = 300,
                Ys = states,
                Xs = days
            };

            using var stream = _plotGenerator.Create(options);

            await _client.SendPhotoAsync(user.ChatId,
                InputFile.FromStream(stream, "mood-report.png"),
                caption: message,
                cancellationToken: cancellationToken
            );
        }

        return NoContent();
    }

    private static (DateTime[] Days, int[] States) GetPlotData(IGrouping<Guid, MoodRecord> group)
    {
        var items = group
            .OrderBy(i => i.CreatedAt)
            .ToList();

        var days = new DateTime[items.Count];
        var states = new int[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            var record = items[i];

            days[i] = record.CreatedAt.WithTime(0, 0);
            states[i] = record.State == MoodState.Good ? 1 : 0;
        }

        return (days, states);
    }
}
