using Eclipse.Domain.Users;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Exporting.Users;

internal sealed class ImportUsersValidator : IImportValidator<ImportUserDto, ImportUsersValidationOptions>
{
    private ImportUsersValidationOptions? _options;

    private readonly IStringLocalizer<ImportUsersValidator> _localizer;

    public ImportUsersValidator(IStringLocalizer<ImportUsersValidator> localizer)
    {
        _localizer = localizer;
    }

    public void Set(ImportUsersValidationOptions instance)
    {
        _options = instance;
    }

    public IEnumerable<ImportUserDto> ValidateAndSetErrors(IEnumerable<ImportUserDto> rows)
    {
        var options = _options ?? new ImportUsersValidationOptions();

        foreach (var row in rows.Where(r => r.CanBeImported()))
        {
            row.Exception = ValidateAndGetError(row, options);

            yield return row;
        }
    }

    private string ValidateAndGetError(ImportUserDto row, ImportUsersValidationOptions options)
    {
        var errors = new List<string>();

        errors.AddRange(
            ValidateUserUniqueness(row, options.Users)
        );

        errors.AddRange(
            ValidateFieldCorrectness(row)
        );

        return errors.Join(", ");
    }

    private IEnumerable<string> ValidateUserUniqueness(ImportUserDto row, List<User> users)
    {
        var user = users.FirstOrDefault(u => u.Id == row.Id);

        if (user is not null)
        {
            yield return _localizer["{0}AlreadyExists{1}{2}", nameof(User), nameof(row.Id), row.Id];
        }

        user = users.FirstOrDefault(u => u.UserName == row.UserName);

        if (!row.UserName.IsNullOrEmpty() && user is not null)
        {
            yield return _localizer["{0}AlreadyExists{1}{2}", nameof(User), nameof(row.UserName), row.UserName];
        }

        user = users.FirstOrDefault(u => u.ChatId == row.ChatId);

        if (user is not null)
        {
            yield return _localizer["{0}AlreadyExists{1}{2}", nameof(User), nameof(row.ChatId), row.ChatId];
        }
    }

    private IEnumerable<string> ValidateFieldCorrectness(ImportUserDto row)
    {
        if (!TimeSpan.TryParse(row.Gmt, out _))
        {
            yield return _localizer["InvalidField{0}{1}", nameof(row.Gmt), row.Gmt];
        }
    }
}
