using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Exporting.Reminders;

internal sealed class ImportRemindersValidator : IImportValidator<ImportReminderDto, ImportRemindersValidationOptions>
{
    private ImportRemindersValidationOptions? _options;

    private readonly IStringLocalizer<ImportRemindersValidator> _localizer;

    public ImportRemindersValidator(IStringLocalizer<ImportRemindersValidator> localizer)
    {
        _localizer = localizer;
    }

    public void Set(ImportRemindersValidationOptions instance)
    {
        _options = instance;
    }

    public IEnumerable<ImportReminderDto> ValidateAndSetErrors(IEnumerable<ImportReminderDto> rows)
    {
        var options = _options ?? new();

        foreach (var row in rows)
        {
            row.Exception = ValidateAndGetException(row, options);
            yield return row;
        }
    }

    private string? ValidateAndGetException(ImportReminderDto row, ImportRemindersValidationOptions options)
    {
        return ValidateReferences(row, options.Users)
            .Concat(
                ValidateFields(row))
            .Join(", ");
    }

    private IEnumerable<string> ValidateReferences(ImportReminderDto row, List<User> users)
    {
        var user = users.FirstOrDefault(u => u.Id == row.UserId);

        if (user is null)
        {
            yield return _localizer["{0}NotFound", nameof(User)];
        }

        var reminder = user?.Reminders.FirstOrDefault(r => r.Id == row.Id);

        if (reminder is not null)
        {
            yield return _localizer["{0}AlreadyExists{1}{2}", nameof(Reminder), nameof(row.Id), row.Id];
        }
    }

    private IEnumerable<string> ValidateFields(ImportReminderDto row)
    {
        if (!TimeOnly.TryParse(row.NotifyAt, out _))
        {
            yield return _localizer["InvalidField{0}{1}", nameof(row.NotifyAt), row.NotifyAt];
        }
    }
}
