using Bogus;

using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.Reports;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Reports;

public sealed class ReportServiceTests
{
    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly IPlotGenerator _plotGenerator;

    private readonly ReportsService _sut;

    public ReportServiceTests()
    {
        _moodRecordRepository = Substitute.For<IMoodRecordRepository>();
        _plotGenerator = Substitute.For<IPlotGenerator>();

        _sut = new(_moodRecordRepository, _plotGenerator);
    }

    [Fact]
    public async Task GetMoodReportAsync_WhenRequested_ThenProperDependenciesCalled()
    {
        var userId = Guid.NewGuid();

        var from = new DateTime(2024, 10, 1);
        var to = new DateTime(2024, 10, 7);
        var count = 7;

        var faker = new Faker();

        var moodRecords = new List<MoodRecord>(count);

        for (int i = 0; i < count; i++)
        {
            moodRecords.Add(new MoodRecord(
                Guid.NewGuid(),
                userId,
                (MoodState)faker.Random.Int(0, 6),
                faker.Date.Between(from, to)
            ));
        }

        _moodRecordRepository.GetByExpressionAsync(
            Arg.Any<Expression<Func<MoodRecord, bool>>>()
        ).Returns(moodRecords);

        var expectedDates = moodRecords.OrderBy(m => m.CreatedAt)
            .Select(m => m.CreatedAt.WithTime(0, 0))
            .ToArray();

        var expectedStates = moodRecords.OrderBy(m => m.CreatedAt)
            .Select(m => m.State.ToScore())
            .ToArray();

        var options = new MoodReportOptions
        {
            From = from,
            To = to,
        };

        using var _ = await _sut.GetMoodReportAsync(userId, options);

        await _moodRecordRepository.Received()
            .GetByExpressionAsync(Arg.Any<Expression<Func<MoodRecord, bool>>>());

        var expectedFrom = expectedDates[0];
        var expectedTo = expectedDates[^1];

        _plotGenerator.Received().Create(
            Arg.Is<PlotOptions<DateTime, int>>(options => options.Title == $"{expectedFrom:dd.MM}-{expectedTo:dd.MM}"
                && options.Left != null
                && options.Left.Label == "Score"
                && options.Left.Values.SequenceEqual(expectedStates)

                && options.Bottom != null
                && options.Bottom.Label == string.Empty
                && options.Bottom.Values.SequenceEqual(expectedDates)

                && options.Width == 600
                && options.Height == 400
            )
        );
    }

    [Fact]
    public async Task GetMoodReportAsync_WhenNoRecordsExists_ThenSetsDefaultEmptyOptions()
    {
        _moodRecordRepository.GetByExpressionAsync(
            Arg.Any<Expression<Func<MoodRecord, bool>>>()
        ).Returns([]);

        var userId = Guid.NewGuid();

        var from = new DateTime(2024, 10, 1);
        var to = new DateTime(2024, 10, 7);

        await _sut.GetMoodReportAsync(userId, new MoodReportOptions { From = from, To = to });

        _plotGenerator.Received().Create(
            Arg.Is<PlotOptions<DateTime, int>>(o => o.Left != null
                && o.Left.Values.SequenceEqual(new int[2])
                && o.Bottom != null
                && o.Bottom.Values.SequenceEqual(new DateTime[] { from, to })
        ));
    }
}
