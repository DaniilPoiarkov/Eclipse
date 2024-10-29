using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.Users;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Users;

public sealed class ImportUsersStrategyTests
{
    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportUserDto, ImportUsersValidationOptions> _validator;

    private readonly ImportUsersStrategy _sut;

    public ImportUsersStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportUserDto, ImportUsersValidationOptions>>();
        _excelManager = Substitute.For<IExcelManager>();

        _sut = new ImportUsersStrategy(new UserManager(_userRepository), _excelManager, _validator);
    }

    [Fact]
    public void Type_WhenChecked_ThenReturnsUserSpecification()
    {
        _sut.Type.Should().Be(ImportType.Users);
    }

    [Fact]
    public async Task ImportAsync_WhenRowsAreValid_ThenProcessedSuccessfully()
    {
        using var stream = new MemoryStream();

        List<ImportUserDto> rows = [
            ImportEntityRowGenerator.User(),
            ImportEntityRowGenerator.User(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.Id));

        _excelManager.Read<ImportUserDto>(stream).Returns(rows);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);

        foreach (var user in users)
        {
            _userRepository.CreateAsync(user).ReturnsForAnyArgs(user);
        }

        var result = await _sut.ImportAsync(stream);

        result.IsSuccess.Should().BeTrue();
        result.FailedRows.Should().BeEmpty();

        await _userRepository.ReceivedWithAnyArgs(rows.Count).CreateAsync(default!);
    }

    [Fact]
    public async Task ImportAsync_WhenRowsAreInvalid_ThenReturnsFailedResult()
    {
        using var stream = new MemoryStream();

        List<ImportUserDto> rows = [
            ImportEntityRowGenerator.User(),
        ];

        _excelManager.Read<ImportUserDto>(stream).Returns(rows);

        var error = "error";

        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows.Select(r =>
        {
            r.Exception = error;
            return r;
        }));

        var result = await _sut.ImportAsync(stream);

        result.IsSuccess.Should().BeFalse();
        result.FailedRows.Should().HaveSameCount(rows);
        result.FailedRows.All(r => r.Exception == error).Should().BeTrue();
    }

    [Fact]
    public async Task ImportAsync_WhenCreationFails_ThenErrorShouldBeSet()
    {
        using var stream = new MemoryStream();

        List<ImportUserDto> rows = [
            ImportEntityRowGenerator.User(),
        ];

        var error = "create user failed";

        _excelManager.Read<ImportUserDto>(stream).Returns(rows);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);
        _userRepository
            .CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Throws(new Exception(error));

        var result = await _sut.ImportAsync(stream);

        result.FailedRows.Should().HaveSameCount(rows);
        result.FailedRows.All(r => r.Exception == error).Should().BeTrue();
    }
}
