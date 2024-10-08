﻿using Eclipse.Application.Contracts.Reports;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Reports;

internal sealed class ReportsService : IReportsService
{
    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly IPlotGenerator _plotGenerator;

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

        var title = days.IsNullOrEmpty()
            ? $"{options.From:dd.MM}-{options.To:dd.MM}"
            : $"{days[0]:dd.MM}-{days[^1]:dd.MM}";

        var option = new PlotOptions<DateTime, int>
        {
            Title = title,
            YAxisTitle = "Score",
            Width = 550,
            Height = 300,
            Ys = states,
            Xs = days
        };

        return _plotGenerator.Create(option);
    }
}
