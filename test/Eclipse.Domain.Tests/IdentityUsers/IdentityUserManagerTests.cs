using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Repositories;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Domain.Tests.IdentityUsers;

public class IdentityUserManagerTests
{
    private readonly IIdentityUserRepository _repository;

    private IdentityUserManager Sut => new(_repository);

    public IdentityUserManagerTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();
    }

    [Fact]
    public async Task CreateAsync_WhenModelValid_ThenCreatedUserReturned()
    {
        var name = "Name";
        var surname = "Surname";
        var chatId = 1;

        var identityUser = IdentityUser.Create(
            Guid.NewGuid(),
            name,
            surname,
            string.Empty,
            chatId);

        _repository.CreateAsync(identityUser).ReturnsForAnyArgs(identityUser);

        var result = await Sut.CreateAsync(name, surname, string.Empty, chatId);

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
