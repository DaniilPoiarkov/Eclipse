﻿using Bogus;

using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.Reminders;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests;

using FluentAssertions;

using MiniExcelLibs;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImortRemindersStrategyTests
{
    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportReminderDto, ImportRemindersValidationOptions> _validator;

    private readonly ImportRemindersStrategy _sut;

    public ImortRemindersStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportReminderDto, ImportRemindersValidationOptions>>();
        _sut = new ImportRemindersStrategy(_userRepository, new ExcelManager(), _validator);
    }

    [Fact]
    public async Task AddReminders_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        var rows = ms.Query<ImportReminderDto>()
            .Where(r => !r.UserId.IsEmpty());

        var faker = new Faker();

        var users = rows.Select(r => r.UserId)
            .Distinct()
            .Select(id => User.Create(id, faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName, faker.Random.Long(min: 0), false))
            .ToList();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs(users);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);

        await _sut.ImportAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);

        foreach (var user in users)
        {
            await _userRepository.Received().UpdateAsync(user);
            user.Reminders.IsNullOrEmpty().Should().BeFalse();
        }
    }

    [Fact]
    public async Task AddReminders_WhenUserNotExist_ThenReportSend()
    {
        using var ms = TestsAssembly.GetValidRemindersExcelFile();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);

        await _sut.ImportAsync(ms);

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
