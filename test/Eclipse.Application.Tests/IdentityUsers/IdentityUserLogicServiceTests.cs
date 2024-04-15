﻿using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers.Services;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserLogicServiceTests
{
    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<IIdentityUserLogicService> _lazySut;

    private IIdentityUserLogicService Sut => _lazySut.Value;

    public IdentityUserLogicServiceTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();
        _lazySut = new Lazy<IIdentityUserLogicService>(
            () => new IdentityUserLogicService(
                new IdentityUserManager(_repository)
            ));
    }

    [Theory]
    [InlineData(-4)]
    [InlineData(4)]
    [InlineData(11)]
    public async Task SetUserGmtTimeAsync_WhenTimeIsValid_ThenUpdatedSuccessfully(int time)
    {
        var user = IdentityUserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<IdentityUser?>(user));

        var utc = DateTime.UtcNow;

        var hour = utc.Hour + time > 23
            ? utc.Hour - (24 - time)
            : utc.Hour + time;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(time, 0, 0);

        var result = await Sut.SetUserGmtTimeAsync(user.Id, currentUserTime);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
        result.IsSuccess.Should().BeTrue();
        result.Value.Gmt.Should().Be(expected);
    }
}
