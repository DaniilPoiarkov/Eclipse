using Bogus;

using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Exceptions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.Reminders;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public class ReminderServiceTests
{
    private readonly IdentityUserManager _userManager = Substitute.For<IdentityUserManager>(Substitute.For<IIdentityUserRepository>());

    private readonly Lazy<IReminderService> _lazySut;
    private IReminderService Sut => _lazySut.Value;

    public ReminderServiceTests()
    {
        _lazySut = new Lazy<IReminderService>(() => new ReminderService(_userManager, new IdentityUserMapper()));
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

        _userManager.FindByIdAsync(identityUser.Id)!.Returns(Task.FromResult(identityUser));

        var result = await Sut.CreateReminderAsync(identityUser.Id, reminderCreateDto);

        result.Reminders.Count.Should().Be(1);

        var reminder = result.Reminders[0];

        reminder.Text.Should().Be(reminderCreateDto.Text);
        reminder.NotifyAt.Should().Be(reminderCreateDto.NotifyAt);
        reminder.UserId.Should().Be(identityUser.Id);
        reminder.Id.Should().NotBeEmpty();

        await _userManager.Received().UpdateAsync(identityUser);
    }

    [Fact]
    public async Task CreateReminderAsync_WhenUserNotExists_ThenObjectNotFoundExceptionThrown()
    {
        var reminderCreateDto = new ReminderCreateDto
        {
            NotifyAt = TimeOnly.FromDateTime(DateTime.UtcNow),
            Text = "Test"
        };

        var action = async () =>
        {
            await Sut.CreateReminderAsync(Guid.NewGuid(), reminderCreateDto);
        };

        await action.Should().ThrowAsync<ObjectNotFoundException>();
        await _userManager.DidNotReceive().UpdateAsync(default!);
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

        _userManager.FindByIdAsync(user.Id)!.Returns(Task.FromResult(user));

        // Act
        var result = await Sut.RemoveRemindersForTime(user.Id, time);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Reminders.Count.Should().Be(1);
        
        var leftReminder = result.Reminders[0];
        leftReminder.Text.Should().Be(text);
        leftReminder.NotifyAt.Should().Be(furtureTime);

        await _userManager.Received().UpdateAsync(user);
    }
}
