using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.IdentityUsers.Exceptions;
using Eclipse.Domain.Shared.IdentityUsers;
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
    public async Task CreateAsync_WhenModelValid_THenCreatedUserReturned()
    {
        var createDto = new IdentityUserCreateDto
        {
            Name = "Name",
            Surname = "Surname",
            ChatId = 1,
            Culture = "en",
            NotificationsEnabled = false,
        };

        var identityUser = new IdentityUser(
            Guid.NewGuid(),
            createDto.Name,
            createDto.Surname,
            createDto.Username,
            createDto.ChatId,
            createDto.Culture,
            createDto.NotificationsEnabled);

        _repository.CreateAsync(IdentityUserGenerator.Generate(1).First()).ReturnsForAnyArgs(identityUser);

        var result = await Sut.CreateAsync(createDto);

        result.Should().NotBeNull();
        result!.Name.Should().Be(createDto.Name);
        result.Surname.Should().Be(createDto.Surname);
        result.Culture.Should().Be(createDto.Culture);
        result.ChatId.Should().Be(createDto.ChatId);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserWithSameUsernameExist_ThenExceptionThrown()
    {
        var returnData = IdentityUserGenerator.Generate(1);

        _repository.GetByExpressionAsync(u => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<IdentityUser>>(returnData));

        var duplicatedData = returnData.First();

        var createDto = new IdentityUserCreateDto
        {
            Name = "Name",
            Surname = "Surname",
            ChatId = duplicatedData.ChatId,
            Culture = "en",
            NotificationsEnabled = false,
            Username = duplicatedData.Username,
        };

        var action = async () =>
        {
            await Sut.CreateAsync(createDto);
        };

        await action.Should().ThrowAsync<DuplicateDataException>();
    }
}
