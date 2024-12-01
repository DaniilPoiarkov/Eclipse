using Bogus;

using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Reminders;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public sealed class ReminderServiceTests
{
    private readonly IUserRepository _repository;

    private readonly ReminderService _sut;

    public ReminderServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _sut = new ReminderService(new UserManager(_repository));
    }

    [Theory]
    [InlineData("test")]
    public async Task CreateAsync_WhenUserExists_ThenReturnsDtoWithCreatedReminder(string text)
    {
        var user = UserGenerator.Get();

        var create = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = text
        };

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs([user]);

        var result = await _sut.CreateAsync(user.ChatId, create);

        result.IsSuccess.Should().BeTrue();

        var dto = result.Value;
        dto.Reminders.Should().ContainSingle();

        var reminder = dto.Reminders[0];

        reminder.Id.Should().NotBeEmpty();
        reminder.UserId.Should().Be(user.Id);
        reminder.Text.Should().Be(create.Text);
        reminder.NotifyAt.Should().Be(create.NotifyAt);

        await _repository.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotExists_ThenErrorReturned()
    {
        var result = await _sut.CreateAsync(Guid.NewGuid(), new ReminderCreateDto());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Theory]
    [InlineData(1)]
    public async Task CreateAsync_WhenUserNotFoundByChatId_ThenErrorReturned(long chatId)
    {
        var result = await _sut.CreateAsync(chatId, new ReminderCreateDto());

        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Theory]
    [InlineData("test")]
    public async Task CreateAsync_WhenValidRequestWithUserId_ThenCreatedSuccessfully(string text)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var create = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
        };

        var result = await _sut.CreateAsync(user.Id, create);

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be(text);
        result.Value.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task CreateAsync_WhenUserWithGivenIdNotExists_ThenErrorReturned()
    {
        var result = await _sut.CreateAsync(Guid.NewGuid(), new ReminderCreateDto());

        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Theory]
    [InlineData("test")]
    public async Task GetAsync_WhenExists_ThenProperModelReturned(string text)
    {
        var user = UserGenerator.Get();
        var expected = user.AddReminder(text, TimeOnly.FromDateTime(DateTime.UtcNow)).ToDto();

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetAsync(user.Id, expected.Id);

        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAsync_WhenUserNotExist_ThenErrorReturned()
    {
        var result = await _sut.GetAsync(Guid.NewGuid(), Guid.NewGuid());
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Fact]
    public async Task GetAsync_WhenReminderNotExist_ThenErrorReturned()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetAsync(user.Id, Guid.NewGuid());
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<Reminder>());
    }

    [Theory]
    [InlineData("test")]
    public async Task GetList_WhenRequested_ThenRemindersReturned(string text)
    {
        var user = UserGenerator.Get();
        var reminder = user.AddReminder(text, new TimeOnly()).ToDto();

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.GetListAsync(user.Id);
        result.Value.Should().BeEquivalentTo([reminder]);
    }

    [Fact]
    public async Task GetList_WhenUserNotExist_ThenErrorReturned()
    {
        var result = await _sut.GetListAsync(Guid.NewGuid());
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Theory]
    [InlineData("test")]
    public async Task RemoveRemindersForTime_WhenUserHaveRemindersForTime_ThenDtoWithoutSpecifiedRemindersReturned(string text)
    {
        var user = UserGenerator.Get();
        var time = new TimeOnly();

        user.AddReminder(text, time);

        var reminder = user.AddReminder(text, time.AddHours(1));

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.RemoveForTimeAsync(user.Id, time);
        result.Value.Reminders.Should().BeEquivalentTo([reminder]);

        await _repository.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task RemoveForTime_WhenUserNotExist_ThenErrorReturned()
    {
        var result = await _sut.RemoveForTimeAsync(Guid.NewGuid(), new TimeOnly());
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }
}
