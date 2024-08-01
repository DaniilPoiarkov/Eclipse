using Eclipse.Application.Authorization;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using NSubstitute;

using System;
using System.Security.Claims;

using Xunit;

namespace Eclipse.Application.Tests.Authorization;

public sealed class LoginManagerTests
{
    private readonly IUserRepository _userRepository;

    private readonly IConfiguration _configuration;

    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly LoginManager _sut;

    public LoginManagerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _configuration = Substitute.For<IConfiguration>();
        _userClaimsPrincipalFactory = Substitute.For<IUserClaimsPrincipalFactory<User>>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _sut = new LoginManager(new UserManager(_userRepository), _configuration, _userClaimsPrincipalFactory, _timeProvider);
    }

    [Fact]
    public async Task LoginAsync_WhenUserAuthenticatesSuccessfully_ThenAccessTokenReturned()
    {
        var user = UserGenerator.Generate(1).First();
        user.SetSignInCode(DateTime.UtcNow);

        _timeProvider.Now.Returns(DateTime.UtcNow);

        _userRepository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(
                Task.FromResult<IReadOnlyList<User>>([user])
            );

        var jwtBearerConfiguration = Substitute.For<IConfigurationSection>();
        _configuration.GetSection("Authorization:JwtBearer").Returns(jwtBearerConfiguration);

        jwtBearerConfiguration["Key"].Returns(new string('x', 50));
        jwtBearerConfiguration["Issuer"].Returns("issuer");
        jwtBearerConfiguration["Audience"].Returns("audience");

        Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            ];
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _userClaimsPrincipalFactory.CreateAsync(user).Returns(principal);


        var result = await _sut.LoginAsync(new LoginRequest { SignInCode = user.SignInCode, UserName = user.UserName });


        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().NotBeEmpty();
        result.Value.AccessToken!.Split('.').Length.Should().Be(3);
        result.Value.Expiration.Should().BePositive();

        await _userClaimsPrincipalFactory.Received(1).CreateAsync(user);
        _configuration.Received(1).GetSection("Authorization:JwtBearer");
        await _userRepository.ReceivedWithAnyArgs(1).GetByExpressionAsync(_ => true);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThenErrorReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(User));

        var result = await _sut.LoginAsync(new LoginRequest { SignInCode = "123456", UserName = "JohnDoe" });

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(expectedError.Code);
        result.Error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public async Task LoginAsync_WhenCodeInvalid_ThenErrorReturned()
    {
        var expectedError = Error.Validation("Account.Login", "Account:InvalidCode");
        var user = UserGenerator.Generate(1).First();

        _userRepository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(
                Task.FromResult<IReadOnlyList<User>>([user])
            );

        var result = await _sut.LoginAsync(new LoginRequest { UserName = user.UserName, SignInCode = new string(user.SignInCode.Reverse().ToArray()) });

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be(expectedError.Code);
        result.Error.Description.Should().Be(expectedError.Description);
    }
}
