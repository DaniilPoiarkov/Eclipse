﻿using Bogus;

using Eclipse.Application.Exporting;
using Eclipse.Application.Exporting.Reminders;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.Reminders;

public sealed class ImortRemindersStrategyTests
{
    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportReminderDto, ImportRemindersValidationOptions> _validator;

    private readonly ImportRemindersStrategy _sut;

    public ImortRemindersStrategyTests()
    {
        _excelManager = Substitute.For<IExcelManager>();
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IImportValidator<ImportReminderDto, ImportRemindersValidationOptions>>();
        _sut = new ImportRemindersStrategy(_userRepository, _excelManager, _validator);
    }

    [Fact]
    public async Task ImportAsync_WhenRecordsValid_ThenImportedSuccessfully()
    {
        using var ms = new MemoryStream();

        IEnumerable<ImportReminderDto> rows = [
            GetRow(),
            GetRow(),
        ];

        var faker = new Faker();

        var users = rows.Select(r => r.UserId)
            .Select(id => User.Create(id, faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName, faker.Random.Long(min: 0), false))
            .ToList();

        _excelManager.Read<ImportReminderDto>(ms).Returns(rows);
        _validator.ValidateAndSetErrors(rows).ReturnsForAnyArgs(rows);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs(users);

        var result = await _sut.ImportAsync(ms);

        result.IsSuccess.Should().BeTrue();
        
        foreach (var user in users)
        {
            user.Reminders.Should().HaveCount(1);
            await _userRepository.Received().UpdateAsync(user);
        }
    }

    [Fact]
    public async Task ImportAsync_WhenUserNotExist_ThenFailureResultReturned()
    {
        using var ms = new MemoryStream();

        var row = GetRow();

        _excelManager.Read<ImportReminderDto>(ms).Returns([row]);
        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);
        _validator.ValidateAndSetErrors([row]).ReturnsForAnyArgs([row]);

        var result = await _sut.ImportAsync(ms);

        result.IsSuccess.Should().BeFalse();
        row.Exception.Should().NotBeEmpty();

        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    private static ImportReminderDto GetRow()
    {
        var faker = new Faker();

        return new ImportReminderDto
        {
            Id = Guid.NewGuid(),
            NotifyAt = "12:00:00",
            Text = faker.Lorem.Word(),
            UserId = Guid.NewGuid()
        };
    }
}
