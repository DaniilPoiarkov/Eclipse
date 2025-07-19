using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Domain.Tests.Users;

public class UserManagerTests
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly UserManager _sut;

    public UserManagerTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new UserManager(_repository, _timeProvider);
    }

    [Theory]
    [InlineData("Name", "Surname", "UserName", 1)]
    [InlineData("John", "Doe", "john.doe", 2)]
    [InlineData("Jane", "Doe", "", 2)]
    public async Task CreateAsync_WhenModelValid_ThenCreatedUserReturned(string name, string surname, string userName, long chatId)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var user = User.Create(Guid.NewGuid(), name, surname, userName, chatId, utcNow, true, true);

        _repository.CreateAsync(user).ReturnsForAnyArgs(user);

        var result = await _sut.CreateAsync(name, surname, userName, chatId);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;
        value.Name.Should().Be(name);
        value.Surname.Should().Be(surname);
        value.ChatId.Should().Be(chatId);
        value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserWithSameChatIdExists_ThenFailureResultReturned()
    {
        var chatId = 1;
        var expectedError = UserDomainErrors.DuplicateData(nameof(CreateUserRequest.ChatId), chatId);

        _repository.CountAsync(u => true)
            .ReturnsForAnyArgs(Task.FromResult(1));

        var result = await _sut.CreateAsync("Name", "Surname", "UserName", chatId);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error!.Description.Should().Be(expectedError.Description);
        error!.Args.Should().BeEquivalentTo(expectedError.Args);
    }
}
