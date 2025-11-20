using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Reports;

internal sealed class ReportsService : IReportsService
{
    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly IUserRepository _userRepository;

    private readonly IPlotGenerator _plotGenerator;

    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<ReportsService> _localizer;

    private const int _width = 600;

    private const int _height = 400;

    private const string _yAxisTitle = "Score";

    private const double _margin = 0.5d;

    public ReportsService(
        IMoodRecordRepository moodRecordRepository,
        IUserRepository userRepository,
        IPlotGenerator plotGenerator,
        ICurrentCulture currentCulture,
        IStringLocalizer<ReportsService> localizer)
    {
        _moodRecordRepository = moodRecordRepository;
        _userRepository = userRepository;
        _plotGenerator = plotGenerator;
        _currentCulture = currentCulture;
        _localizer = localizer;
    }

    public async Task<MemoryStream> GetMoodReportAsync(Guid userId, MoodReportOptions options, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken) 
            ?? throw new ArgumentException("User not found.", nameof(userId));

        var records = await _moodRecordRepository.GetByExpressionAsync(
            mr => mr.UserId == userId
                && mr.CreatedAt >= options.From
                && mr.CreatedAt <= options.To,
            cancellationToken
        );

        var days = new DateTime[records.Count];
        var states = new int[records.Count];

        var tuples = records.OrderBy(r => r.CreatedAt)
            .Select((record, index) => (record, index));

        foreach (var (record, index) in tuples)
        {
            days[index] = record.CreatedAt.WithTime(0, 0);
            states[index] = record.State.ToScore();
        }

        if (days.IsNullOrEmpty())
        {
            days = [options.From, options.To];
            states = new int[days.Length];
        }

        var title = $"{days[0]:dd.MM}-{days[^1]:dd.MM}";

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var option = new PlotOptions<DateTime, int>
        {
            Bottom = new AxisOptions<DateTime>
            {
                Values = days,
            },
            Left = new AxisOptions<int>
            {
                Values = states,
                Label = _localizer[_yAxisTitle],
                Min = MoodState.Bad.ToScore() - _margin,
                Max = MoodState.Good.ToScore() + _margin,
            },
            Title = title,
            Width = _width,
            Height = _height,
        };

        return _plotGenerator.Create(option);
    }
}
