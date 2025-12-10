using Bogus;

using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.MoodRecords;
using Eclipse.Common.Plots;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords;

public sealed class MoodReportServiceTests
{
    private readonly IMoodRecordRepository _moodRecordRepository;

    private readonly IUserRepository _userRepository;

    private readonly IPlotGenerator _plotGenerator;

    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<MoodReportService> _localizer;

    private readonly MoodReportService _sut;

    public MoodReportServiceTests()
    {
        _moodRecordRepository = Substitute.For<IMoodRecordRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _plotGenerator = Substitute.For<IPlotGenerator>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _localizer = Substitute.For<IStringLocalizer<MoodReportService>>();

        _sut = new(_moodRecordRepository, _userRepository, _plotGenerator, _currentCulture, _localizer);
    }

    [Fact]
    public async Task GetMoodReportAsync_WhenRequested_ThenProperDependenciesCalled()
    {
        var user = UserGenerator.Get();
        _userRepository.FindAsync(user.Id).Returns(user);

        _localizer["Score"].Returns(new LocalizedString("Score", "Localized score text"));

        var from = new DateTime(2024, 10, 1);
        var to = new DateTime(2024, 10, 7);
        var count = 7;

        var faker = new Faker();

        var moodRecords = new List<MoodRecord>(count);

        for (int i = 0; i < count; i++)
        {
            moodRecords.Add(new MoodRecord(
                Guid.NewGuid(),
                user.Id,
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

        using var _ = await _sut.GetAsync(user.Id, options);

        await _moodRecordRepository.Received()
            .GetByExpressionAsync(Arg.Any<Expression<Func<MoodRecord, bool>>>());

        _currentCulture.Received().UsingCulture(user.Culture);

        var expectedFrom = expectedDates[0];
        var expectedTo = expectedDates[^1];

        _plotGenerator.Received().Create(
            Arg.Is<PlotOptions<DateTime, int>>(options => options.Title == $"{expectedFrom:dd.MM}-{expectedTo:dd.MM}"
                && options.Left != null
                && options.Left.Label == "Localized score text"
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
        var user = UserGenerator.Get();
        _userRepository.FindAsync(user.Id).Returns(user);

        _moodRecordRepository.GetByExpressionAsync(
            Arg.Any<Expression<Func<MoodRecord, bool>>>()
        ).Returns([]);

        var from = new DateTime(2024, 10, 1);
        var to = new DateTime(2024, 10, 7);

        await _sut.GetAsync(user.Id, new MoodReportOptions { From = from, To = to });

        _plotGenerator.Received().Create(
            Arg.Is<PlotOptions<DateTime, int>>(o => o.Left != null
                && o.Left.Values.SequenceEqual(new int[2])
                && o.Bottom != null
                && o.Bottom.Values.SequenceEqual(new DateTime[] { from, to })
        ));
    }

    [Fact]
    public async Task GetMoodReportAsync_WhenUserNotFound_ThenThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        var act = () => _sut.GetAsync(userId, new MoodReportOptions());

        var exception = await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("User not found. (Parameter 'userId')");
    }
}
