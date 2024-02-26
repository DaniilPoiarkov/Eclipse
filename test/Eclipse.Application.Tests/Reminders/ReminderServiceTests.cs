using Bogus;

using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.Reminders;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public class ReminderServiceTests
{
    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<IReminderService> _lazySut;
    private IReminderService Sut => _lazySut.Value;

    public ReminderServiceTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();
        _lazySut = new Lazy<IReminderService>(
            () => new ReminderService(
                new IdentityUserManager(_repository),
                new IdentityUserMapper()
            ));
    }

    [Fact]
    public async Task CreateReminderAsync_WhenUserExists_ThenReturnesDtoWithCreatedReminder()
    {
        var identityUser = IdentityUserGenerator.Generate(1).First();

        var reminderCreateDto = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = "Test"
        };

        _repository.FindAsync(identityUser.Id)!.Returns(Task.FromResult(identityUser));

        var result = await Sut.CreateReminderAsync(identityUser.Id, reminderCreateDto);

        result.IsSuccess.Should().BeTrue();

        var user = result.Value;
        user.Reminders.Count.Should().Be(1);

        var reminder = user.Reminders[0];
        reminder.Text.Should().Be(reminderCreateDto.Text);
        reminder.NotifyAt.Should().Be(reminderCreateDto.NotifyAt);
        reminder.UserId.Should().Be(identityUser.Id);
        reminder.Id.Should().NotBeEmpty();

        await _repository.Received().UpdateAsync(identityUser);
    }

    [Fact]
    public async Task CreateReminderAsync_WhenUserNotExists_ThenEntityNotFoundExceptionThrown()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(IdentityUser));

        var reminderCreateDto = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = "Test"
        };

        var result = await Sut.CreateReminderAsync(Guid.NewGuid(), reminderCreateDto);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(error.Code);
        error.Description.Should().Be(error.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
        error.Type.Should().Be(expectedError.Type);
        await _repository.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task RemoveRemindersForTime_WhenUserHaveRemindersForTime_ThenDtoWithoutSpecifiedRemindersReturned()
    {
        // Arrange
        var user = IdentityUserGenerator.Generate(1).First();

        var faker = new Faker();

        var time = TimeOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < 3; i++)
        {
            user.AddReminder(faker.Lorem.Word(), time);
        }

        var text = faker.Lorem.Word();
        var furtureTime = time.AddHours(1);

        user.AddReminder(text, furtureTime);

        _repository.FindAsync(user.Id)!.Returns(Task.FromResult(user));

        // Act
        var result = await Sut.RemoveRemindersForTime(user.Id, time);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Reminders.Count.Should().Be(1);
        
        var leftReminder = result.Reminders[0];
        leftReminder.Text.Should().Be(text);
        leftReminder.NotifyAt.Should().Be(furtureTime);

        await _repository.Received().UpdateAsync(user);
    }
}
