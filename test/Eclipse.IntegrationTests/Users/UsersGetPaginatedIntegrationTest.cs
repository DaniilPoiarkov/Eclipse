using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using System.Net.Http.Json;

namespace Eclipse.IntegrationTests.Users;

public sealed class UsersGetPaginatedIntegrationTest : IntegrationTestBase
{
    public UsersGetPaginatedIntegrationTest(TestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetPaginated_WhenFiltrationApplied_ThenProperRecordsReturned()
    {
        // Arrange
        var entitiesCount = 25;
        var nameOption = "a";
        var page = 1;
        var pageSize = 10;

        var users = await ArrangeData(entitiesCount);
        var expected = GetExpectedValues(users, nameOption, page, pageSize);

        var request = new PaginationRequest<GetUsersRequest>()
        {
            Page = page,
            PageSize = pageSize,
            Options = new GetUsersRequest
            {
                Name = nameOption,
            }
        };

        // Act
        using var response = await Client.PostAsJsonAsync("/api/users/paginated", request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var json = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<PaginatedList<IdentityUserSlimDto>>(json);

        // Assert
        list.Should().NotBeNull();

        list!.Items.All(u => Match(u.Name, nameOption)).Should().BeTrue();

        list.Count.Should().Be(expected.Count);
        list.TotalCount.Should().Be(expected.TotalCount);
        list.Pages.Should().Be(expected.Pages);
    }

    private static bool Match(string name, string option)
    {
        return name.Contains(option, StringComparison.OrdinalIgnoreCase);
    }

    private static ExpectedValues GetExpectedValues(IdentityUser[] users, string nameRequirement, int page, int pageSize)
    {
        var expectedTotalCount = users.Count(u => Match(u.Name, nameRequirement));

        var expectedCount = expectedTotalCount > pageSize
            ? pageSize
            : expectedTotalCount;

        var expectedPages = expectedTotalCount > pageSize
            ? (int)Math.Ceiling((double)expectedTotalCount / pageSize)
            : page;

        return new ExpectedValues(expectedTotalCount, expectedCount, expectedPages);
    }

    private async Task<IdentityUser[]> ArrangeData(int count)
    {
        var insertings = new List<Task<Result<IdentityUser>>>(count);
        var manager = Scope.ServiceProvider.GetRequiredService<IdentityUserManager>();

        for (int i = 0; i < count; i++)
        {
            var name = Faker.Name.FirstName();
            var surname = Faker.Name.LastName();
            var userName = Faker.Internet.UserName(name, surname);
            var chatId = Faker.Random.Long(min: 0);

            insertings.Add(
                manager.CreateAsync(name, surname, userName, chatId)
            );
        }

        return (await Task.WhenAll(insertings))
            .Select(r => r.Value)
            .ToArray();
    }

    private record ExpectedValues(int TotalCount, int Count, int Pages);
}
