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
                faker.Random.Bool() ? MoodState.Good : MoodState.Bad,
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
            .Select(m => m.State == MoodState.Good ? 1 : 0)
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
                    && options.YAxisTitle == "Score"
                    && options.XAxisTitle == null
                    && options.Width == 550
                    && options.Height == 300
                    && options.Xs.SequenceEqual(expectedDates)
                    && options.Ys.SequenceEqual(expectedStates))
        );
    }
}
