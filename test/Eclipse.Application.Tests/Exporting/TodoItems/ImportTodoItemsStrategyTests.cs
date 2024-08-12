﻿using Bogus;

using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests;

using FluentAssertions;

using MiniExcelLibs;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

public sealed class ImportTodoItemsStrategyTests
{
    private readonly IUserRepository _userRepository;

    private readonly ImportTodoItemsStrategy _sut;

    public ImportTodoItemsStrategyTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sut = new ImportTodoItemsStrategy(_userRepository, new ExcelManager());
    }

    [Fact]
    public async Task AddTodoItems_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        var reminders = stream.Query<ImportTodoItemDto>()
            .Where(item => !item.UserId.IsEmpty());

        var faker = new Faker();

        var users = reminders.Select(r => r.UserId)
            .Distinct()
            .Select(id => User.Create(id, faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName, faker.Random.Long(min: 0), false))
            .ToList();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs(users);
        
        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);

        foreach (var user in users)
        {
            await _userRepository.Received().UpdateAsync(user);
            user.TodoItems.IsNullOrEmpty().Should().BeFalse();
        }
    }

    [Fact]
    public async Task AddTodoItems_WhenUserNotExist_ThenNoUpdatePerformed()
    {
        using var stream = TestsAssembly.GetValidTodoItemsExcelFile();

        _userRepository.GetByExpressionAsync(_ => true).ReturnsForAnyArgs([]);

        await _sut.ImportAsync(stream);

        await _userRepository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}