using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

public sealed class ImportTodoItemsValidatorTests
{
    private readonly IStringLocalizer<ImportTodoItemsValidator> _localizer;

    private readonly ImportTodoItemsValidator _sut;

    public ImportTodoItemsValidatorTests()
    {
        _localizer = new EmptyStringLocalizer<ImportTodoItemsValidator>();
        _sut = new(_localizer);
    }

    [Fact]
    public void ValidateAndSetErrorsAsync_WhenRecordsValid_ThenNoErrorsSet()
    {
        IEnumerable<ImportTodoItemDto> rows = [
            ImportEntityRowGenerator.TodoItem(),
            ImportEntityRowGenerator.TodoItem(),
        ];

        var users = UserGenerator.GetWithIds(rows.Select(r => r.UserId)).ToList();

        var options = new ImportTodoItemsValidationOptions
        {
            Users = users,
        };

        _sut.Set(options);

        var result = _sut.ValidateAndSetErrors(rows);

        foreach (var row in result)
        {
            row.Exception.Should().BeEmpty();
        }
    }
}
