using Eclipse.Domain.Shared.TodoItems;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Exporting.TodoItems;

internal sealed class ImportTodoItemsValidator : IImportValidator<ImportTodoItemDto, ImportTodoItemsValidationOptions>
{
    private ImportTodoItemsValidationOptions? _options;

    private readonly IStringLocalizer<ImportTodoItemsValidator> _localizer;

    public ImportTodoItemsValidator(IStringLocalizer<ImportTodoItemsValidator> localizer)
    {
        _localizer = localizer;
    }

    public void Set(ImportTodoItemsValidationOptions instance)
    {
        _options = instance;
    }

    public IEnumerable<ImportTodoItemDto> ValidateAndSetErrors(IEnumerable<ImportTodoItemDto> rows)
    {
        var options = _options ?? new ImportTodoItemsValidationOptions();

        foreach (var row in rows.Where(r => r.CanBeImported()))
        {
            row.Exception = ValidateAndGetError(row, options);
            yield return row;
        }
    }

    private string? ValidateAndGetError(ImportTodoItemDto row, ImportTodoItemsValidationOptions options)
    {
        var errors = new List<string>();

        errors.AddRange(
            ValidateReferences(row, options.Users)
        );
        errors.AddRange(
            ValidateFields(row)
        );

        return errors.Join(", ");
    }

    private IEnumerable<string> ValidateReferences(ImportTodoItemDto row, List<User> users)
    {
        var user = users.FirstOrDefault(u => u.Id == row.UserId);

        if (user is null)
        {
            yield return _localizer["{0}NotFound", nameof(User)];
        }

        var existing = user?.TodoItems.FirstOrDefault(i => i.Id == row.Id);

        if (existing is not null)
        {
            yield return _localizer["{0}AlreadyExists{1}{2}", nameof(TodoItem), nameof(row.Id), row.Id];
        }

        if (user?.TodoItems.Count >= TodoItemConstants.Limit)
        {
            yield return _localizer["TodoItem:Limit", TodoItemConstants.Limit];
        }
    }

    private IEnumerable<string> ValidateFields(ImportTodoItemDto row)
    {
        if (row.Text.Length > TodoItemConstants.MaxLength)
        {
            yield return _localizer["TodoItem:MaxLength", TodoItemConstants.MaxLength];
        }

        if (row.Text.Length < TodoItemConstants.MinLength)
        {
            yield return _localizer["TodoItem:Empty"];
        }
    }
}
