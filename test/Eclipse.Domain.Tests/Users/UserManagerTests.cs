using Eclipse.Domain.Users;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Domain.Tests.Users;

public class UserManagerTests
{
    private readonly IUserRepository _repository;

    private UserManager Sut => new(_repository);

    public UserManagerTests()
    {
        _repository = Substitute.For<IUserRepository>();
    }

    [Theory]
    [InlineData("Name", "Surname", "UserName", 1)]
    [InlineData("John", "Doe", "john.doe", 2)]
    [InlineData("Jane", "Doe", "", 2)]
    public async Task CreateAsync_WhenModelValid_ThenCreatedUserReturned(string name, string surname, string userName, long chatId)
    {
        var user = User.Create(Guid.NewGuid(), name, surname, userName, chatId, true);

        _repository.CreateAsync(user).ReturnsForAnyArgs(user);

        var result = await Sut.CreateAsync(name, surname, userName, chatId);

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
        var expectedError = UserDomainErrors.DuplicateData(nameof(chatId), chatId);

        _repository.CountAsync(u => true)
            .ReturnsForAnyArgs(Task.FromResult(1));

        var result = await Sut.CreateAsync("Name", "Surname", "UserName", chatId);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error!.Description.Should().Be(expectedError.Description);
        error!.Args.Should().BeEquivalentTo(expectedError.Args);
    }
}
