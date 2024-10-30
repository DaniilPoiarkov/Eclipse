using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.MoodRecords;
using Eclipse.Common.Clock;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords;

public sealed class MoodRecordsServiceTests
{
    private readonly IMoodRecordRepository _repository;

    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly MoodRecordsService _sut;

    public MoodRecordsServiceTests()
    {
        _repository = Substitute.For<IMoodRecordRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        var manager = new UserManager(_userRepository);

        _sut = new(_repository, manager, _timeProvider);
    }

    [Fact]
    public async Task GetListAsync_WhenCalled_ThenRecordsReturned()
    {
        var record = new MoodRecord(Guid.NewGuid(), Guid.NewGuid(), MoodState.Good, DateTime.UtcNow);

        _repository.GetByExpressionAsync(Arg.Any<Expression<Func<MoodRecord, bool>>>())
            .Returns([record]);

        var result = await _sut.GetListAsync(record.UserId);

        result.Count.Should().Be(1);
        result[0].Id.Should().Be(record.Id);
        result[0].UserId.Should().Be(record.UserId);
        result[0].State.Should().Be(record.State);
        result[0].CreatedAt.Should().Be(record.CreatedAt);
    }

    [Theory]
    [InlineData(MoodState.Bad)]
    [InlineData(MoodState.Good)]
    public async Task CreateOrUpdateAsync_WhenUserExists_ThenCreatedMoodRecordReturned(MoodState state)
    {
        var user = UserGenerator.Get();

        _userRepository.FindAsync(user.Id).Returns(user);

        var utcNow = DateTime.UtcNow;

        _timeProvider.Now.Returns(utcNow);

        var model = new CreateMoodRecordDto
        {
            State = state
        };

        var result = await _sut.CreateOrUpdateAsync(user.Id, model);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;

        value.Id.Should().NotBeEmpty();
        value.UserId.Should().Be(user.Id);
        value.State.Should().Be(state);
        value.CreatedAt.Should().Be(utcNow);
    }

    [Theory]
    [InlineData(MoodState.Good, MoodState.SlightlyGood)]
    [InlineData(MoodState.Good, MoodState.Good)]
    [InlineData(MoodState.Bad, MoodState.SlightlyGood)]
    public async Task CreateOrUpdateAsync_WhenMoodRecordExists_ThenNoNewRecordCreated(MoodState initialState, MoodState newState)
    {
        var userId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;

        var moodRecord = new MoodRecord(Guid.NewGuid(), userId, initialState, utcNow);

        _repository.FindForDateAsync(userId, utcNow).Returns(moodRecord);

        var model = new CreateMoodRecordDto
        {
            State = newState,
        };

        var result = await _sut.CreateOrUpdateAsync(userId, model);

        await _repository.DidNotReceiveWithAnyArgs().CreateAsync(moodRecord);

        result.IsSuccess.Should().BeTrue();

        var actual = result.Value;

        actual.State.Should().Be(newState);
        actual.UserId.Should().Be(userId);
        actual.CreatedAt.Should().Be(utcNow);
        actual.Id.Should().Be(moodRecord.Id);
    }

    [Fact]
    public async Task CreateOrUpdateAsync_WhenUserNotExists_ThenFailedResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(User));

        var model = new CreateMoodRecordDto
        {
            State = MoodState.Good
        };

        var result = await _sut.CreateOrUpdateAsync(Guid.NewGuid(), model);

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(expectedError, result.Error);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecordExist_AndHasSameUserId_ThenRecordReturned()
    {
        var userId = Guid.NewGuid();

        var utcNow = DateTime.UtcNow;

        var moodRecord = new MoodRecord(Guid.NewGuid(), userId, MoodState.Good, utcNow);

        _repository.FindAsync(moodRecord.Id).Returns(moodRecord);

        var result = await _sut.GetByIdAsync(userId, moodRecord.Id);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;

        value.Id.Should().Be(moodRecord.Id);
        value.UserId.Should().Be(userId);
        value.State.Should().Be(MoodState.Good);
        value.CreatedAt.Should().Be(utcNow);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRecordExist_AndHasAnotherUserId_ThenFailedResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(MoodRecord));

        var moodRecord = new MoodRecord(Guid.NewGuid(), Guid.NewGuid(), MoodState.Good, DateTime.UtcNow);

        _repository.FindAsync(moodRecord.Id).Returns(moodRecord);

        var result = await _sut.GetByIdAsync(Guid.NewGuid(), moodRecord.Id);

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(expectedError, result.Error);
    }
}
