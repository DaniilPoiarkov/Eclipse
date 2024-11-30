using Bogus;

using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Reminders;
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

    [Fact]
    public async Task CreateAsync_WhenUserExists_ThenReturnsDtoWithCreatedReminder()
    {
        var user = UserGenerator.Generate(1).First();

        var reminderCreateDto = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = "Test"
        };

        _repository.GetByExpressionAsync(_ => true)!
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        var result = await _sut.CreateAsync(user.ChatId, reminderCreateDto);

        result.IsSuccess.Should().BeTrue();

        var userDto = result.Value;
        userDto.Reminders.Count.Should().Be(1);

        var reminder = userDto.Reminders[0];
        reminder.Text.Should().Be(reminderCreateDto.Text);
        reminder.NotifyAt.Should().Be(reminderCreateDto.NotifyAt);
        reminder.UserId.Should().Be(user.Id);
        reminder.Id.Should().NotBeEmpty();

        await _repository.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task CreateReminderAsync_WhenUserNotExists_ThenEntityNotFoundExceptionThrown()
    {
        var expectedError = DefaultErrors.EntityNotFound<User>();

        var reminderCreateDto = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = "Test"
        };

        var result = await _sut.CreateAsync(Guid.NewGuid(), reminderCreateDto);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(error.Code);
        error.Description.Should().Be(error.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
        error.Type.Should().Be(expectedError.Type);
        await _repository.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task RemoveRemindersForTime_WhenUserHaveRemindersForTime_ThenDtoWithoutSpecifiedRemindersReturned()
    {
        // Arrange
        var user = UserGenerator.Generate(1).First();

        var faker = new Faker();

        var time = TimeOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < 3; i++)
        {
            user.AddReminder(faker.Lorem.Word(), time);
        }

        var text = faker.Lorem.Word();
        var futureTime = time.AddHours(1);

        user.AddReminder(text, futureTime);

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<User?>(user));

        // Act
        var result = await _sut.RemoveForTimeAsync(user.Id, time);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var dto = result.Value;
        dto.Should().NotBeNull();
        dto.Id.Should().Be(dto.Id);
        dto.Reminders.Count.Should().Be(1);

        var leftReminder = dto.Reminders[0];
        leftReminder.Text.Should().Be(text);
        leftReminder.NotifyAt.Should().Be(futureTime);

        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [InlineData(1)]
    public async Task CreateAsync_WhenUserNotFoundByChatId_ThenErrorReturned(long chatId)
    {
        _repository.GetByExpressionAsync(u => u.ChatId == chatId).Returns([]);

        var result = await _sut.CreateAsync(chatId, new ReminderCreateDto());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotFoundById_ThenErrorReturned()
    {
        var id = Guid.NewGuid();
        _repository.GetByExpressionAsync(u => u.Id == id).Returns([]);

        var result = await _sut.CreateAsync(id, new ReminderCreateDto());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }
}
