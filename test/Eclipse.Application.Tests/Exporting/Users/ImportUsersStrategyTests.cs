using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.Users;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests;

using Microsoft.Extensions.Localization;

using MiniExcelLibs;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Users;

public sealed class ImportUsersStrategyTests
{
    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportUserDto, ImportUsersValidationOptions> _validator;

    private readonly ImportUsersStrategy _sut;

    public ImportUsersStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportUserDto, ImportUsersValidationOptions>>();

        _sut = new ImportUsersStrategy(new UserManager(_userRepository), new ExcelManager(), _validator);
    }

    [Fact]
    public async Task AddUsers_WhenFileIsValid_ThenProcessedSuccessfully()
    {
        using var stream = TestsAssembly.GetValidUsersExcelFile();

        var rows = stream.Query<ImportUserDto>();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);
        _validator.ValidateAndSetErrors(rows).Returns(rows);

        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().CreateAsync(default!);
    }

    [Fact]
    public async Task AddUsers_WhenRecordsInvalid_ThenReportSend()
    {
        using var stream = TestsAssembly.GetInvalidUsersExcelFile();

        await _sut.ImportAsync(stream);

        await _userRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!);
    }
}
