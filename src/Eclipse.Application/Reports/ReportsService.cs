using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Reports;

internal sealed class ReportsService : IReportsService
{
    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly IPlotGenerator _plotGenerator;

    private static readonly int _width = 550;

    private static readonly int _height = 300;

    private static readonly string _yAxisTitle = "Score";

    private static readonly double _margin = 0.5d;

    public ReportsService(
        IMoodRecordRepository moodRecordRepository,
        IPlotGenerator plotGenerator)
    {
        _moodRecordRepository = moodRecordRepository;
        _plotGenerator = plotGenerator;
    }

    public async Task<MemoryStream> GetMoodReportAsync(Guid userId, MoodReportOptions options, CancellationToken cancellationToken = default)
    {
        var records = await _moodRecordRepository.GetByExpressionAsync(
            mr => mr.UserId == userId
                && mr.CreatedAt >= options.From
                && mr.CreatedAt <= options.To,
            cancellationToken
        );

        var days = new DateTime[records.Count];
        var states = new int[records.Count];

        var index = 0;

        foreach (var record in records.OrderBy(r => r.CreatedAt))
        {
            days[index] = record.CreatedAt.WithTime(0, 0);
            states[index] = record.State.ToScore();

            index++;
        }

        if (days.IsNullOrEmpty())
        {
            days = [options.From, options.To];
            states = new int[2];
        }

        var title = $"{days[0]:dd.MM}-{days[^1]:dd.MM}";

        var option = new PlotOptions<DateTime, int>
        {
            Bottom = new AxisOptions<DateTime>
            {
                Values = days,
            },
            Left = new AxisOptions<int>
            {
                Values = states,
                Label = _yAxisTitle,
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
