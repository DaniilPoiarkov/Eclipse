using Bogus;

using Eclipse.Application.Exporting;
using Eclipse.Application.Tests.Exporting.Reminders;
using Eclipse.Application.Tests.Exporting.TodoItems;
using Eclipse.Application.Tests.Exporting.Users;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests.Generators;

using FluentAssertions;

using MiniExcelLibs;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Exporting;

public sealed class ExportServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly ExportService _sut;

    public ExportServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();

        _sut = new ExportService(new ExcelManager(), _userRepository);
    }

    [Fact]
    public async Task GetUsers_WhenExported_ThenProperDataReturned()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetAllAsync().ReturnsForAnyArgs(users);

        using var stream = await _sut.GetUsersAsync();

        var result = stream.Query<ExportedUser>();

        Compare(users, result, new UserRowComparer()).Should().BeTrue();
    }

    [Fact]
    public async Task GetTodoItems_WhenExported_ThenProperDataReturned()
    {
        var user = UserGenerator.Get();

        var faker = new Faker();

        for (int i = 0; i < 5; i++)
        {
            user.AddTodoItem(faker.Lorem.Sentence(), faker.Date.Past());
        }

        _userRepository.GetAllAsync().Returns([user]);

        using var stream = await _sut.GetTodoItemsAsync();

        var result = stream.Query<ExportedTodoItem>();

        Compare(user.TodoItems, result, new TodoItemRowComparer()).Should().BeTrue();
    }

    [Fact]
    public async Task GetReminders_WhenExported_ThenProperDataReturned()
    {
        var user = UserGenerator.Get();

        var count = 5;

        var faker = new Faker();

        for (int i = 0; i < count; i++)
        {
            user.AddReminder(
                faker.Lorem.Sentence(),
                faker.Date.BetweenTimeOnly(new TimeOnly(0, 0), new TimeOnly(23, 59))
            );
        }

        _userRepository.GetAllAsync().Returns([user]);

        using var stream = await _sut.GetRemindersAsync();

        var result = stream.Query<ExportedReminder>();

        Compare(user.Reminders, result, new ReminderRowComparer()).Should().BeTrue();
    }

    private static bool Compare<TEntity, TRow>(IEnumerable<TEntity> entities, IEnumerable<TRow> rows, IExportRowComparer<TEntity, TRow> comparer)
        where TEntity : Entity
        where TRow : ExportedRow
    {
        foreach (var entity in entities)
        {
            var actual = rows.FirstOrDefault(r => r.Id == entity.Id);

            if (actual is null)
            {
                return false;
            }

            var result = comparer.Compare(entity, actual);

            if (!result)
            {
                return false;
            }
        }

        return true;
    }
}
