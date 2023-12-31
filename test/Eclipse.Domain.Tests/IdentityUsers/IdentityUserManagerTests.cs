﻿using Eclipse.Domain.IdentityUsers;
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
        var name = "Name";
        var surname = "Surname";
        var chatId = 1;

        var identityUser = IdentityUser.Create(
            Guid.NewGuid(),
            name,
            surname,
            string.Empty,
            chatId);

        _repository.CreateAsync(IdentityUserGenerator.Generate(1).First()).ReturnsForAnyArgs(identityUser);

        var result = await Sut.CreateAsync(name, surname, string.Empty, chatId);

        result.Should().NotBeNull();
        result!.Name.Should().Be(name);
        result.Surname.Should().Be(surname);
        result.ChatId.Should().Be(chatId);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenUserWithSameUsernameExist_ThenExceptionThrown()
    {
        var returnData = IdentityUserGenerator.Generate(1);

        _repository.GetByExpressionAsync(u => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<IdentityUser>>(returnData));

        var duplicatedData = returnData.First();

        var action = async () =>
        {
            await Sut.CreateAsync("Name", "Surname", duplicatedData.Username, duplicatedData.ChatId);
        };

        await action.Should().ThrowAsync<DuplicateDataException>();
    }
}
