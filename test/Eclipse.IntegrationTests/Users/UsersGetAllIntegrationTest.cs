using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Domain.IdentityUsers;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using System.Net.Http.Json;

namespace Eclipse.IntegrationTests.Users;

public sealed class UsersGetAllIntegrationTest : IntegrationTestBase
{
    public UsersGetAllIntegrationTest(WebAppFactoryWithTestcontainers factory)
        : base(factory)
    {
        AddAppAuthorizationHeader();
    }

    //[Fact]
    public async Task GetAll_WhenCalled_ThenAllUsersReturned()
    {
        var name = Faker.Name.FirstName();
        var surname = Faker.Name.LastName();
        var userName = Faker.Internet.UserName();
        var chatId = Faker.Random.Long(min: 0);

        var expectedCount = 1;

        var manager = Scope.ServiceProvider.GetRequiredService<IdentityUserManager>();

        _ = await manager.CreateAsync(name, surname, userName, chatId);

        var users = await Client.GetFromJsonAsync<List<IdentityUserSlimDto>>("api/users");

        users.Should().NotBeNullOrEmpty();
        users!.Count.Should().Be(expectedCount);
        var user = users[0];

        user.Name.Should().Be(name);
        user.Surname.Should().Be(surname);
        user.Username.Should().Be(userName);
        user.ChatId.Should().Be(chatId);
        user.Id.Should().NotBeEmpty();
    }
}
