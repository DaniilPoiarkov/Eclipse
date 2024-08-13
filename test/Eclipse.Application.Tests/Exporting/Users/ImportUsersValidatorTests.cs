using Eclipse.Application.Exporting.Users;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Users;

public sealed class ImportUsersValidatorTests
{
    private readonly IStringLocalizer<ImportUsersValidator> _localizer;

    private readonly ImportUsersValidator _sut;

    public ImportUsersValidatorTests()
    {
        _localizer = new EmptyStringLocalizer<ImportUsersValidator>();
        _sut = new ImportUsersValidator(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrors_WhenUserExists_ThenErrorSet()
    {
        IEnumerable<ImportUserDto> rows = [
            ImportEntityRowGenerator.User(),
            ImportEntityRowGenerator.User(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.Id)).ToList();

        var options = new ImportUsersValidationOptions
        {
            Users = users
        };

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors(rows);

        foreach (var row in result)
        {
            row.Exception.Should().Be(
                _localizer["{0}AlreadyExists{1}{2}", nameof(User), nameof(row.Id), row.Id]
            );
        }
    }

    [Fact]
    public void ValidateAndSetErrors_WhenGmtIsInvalid_ThenErrorSet()
    {
        var row = ImportEntityRowGenerator.User(gmt: "qwerty");

        foreach (var result in _sut.ValidateAndSetErrors([row]))
        {
            result.Exception.Should().Be(_localizer["InvalidField{0}{1}", nameof(row.Gmt), row.Gmt]);
        }
    }
}
