using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests.Generators;

using FluentAssertions;

using MiniExcelLibs;

using Newtonsoft.Json;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting;

public sealed class ExportServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly Lazy<IExportService> _sut;

    private IExportService Sut => _sut.Value;

    public ExportServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();

        _sut = new(() => new ExportService(new ExcelManager(), _userRepository));
    }

    [Fact]
    public async Task GetUsers_WhenExported_ThenProperDataReturned()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetAllAsync().ReturnsForAnyArgs(users);

        var stream = await Sut.GetUsersAsync();

        var result = stream.Query<ExportedUser>();

        CompareUsers(users, result).Should().BeTrue();
    }

    private static bool CompareUsers(List<User> expected, IEnumerable<ExportedUser> actual)
    {
        foreach (var user in expected)
        {
            var recieved = actual.FirstOrDefault(a => a.Id == user.Id);

            if (recieved is null)
            {
                return false;
            }

            var equal = user.Id == recieved.Id
                && user.Name == recieved.Name
                && user.Surname == recieved.Surname
                && user.Gmt == TimeSpan.Parse(recieved.Gmt)
                && user.UserName == recieved.UserName
                && user.ChatId == recieved.ChatId
                && user.Culture == recieved.Culture
                && user.NotificationsEnabled == recieved.NotificationsEnabled;

            if (!equal)
            {
                return false;
            }
        }

        return true;
    }
}

internal class ExportedUser
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }

    public string Gmt { get; set; } = string.Empty;
}
