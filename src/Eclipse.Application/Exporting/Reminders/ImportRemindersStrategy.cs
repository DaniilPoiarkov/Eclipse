using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Reminders;

internal sealed class ImportRemindersStrategy : IImportStrategy
{
    public ImportType Type => ImportType.Reminders;


    private readonly IUserRepository _userRepository;

    private readonly IExcelManager _excelManager;

    private readonly IImportValidator<ImportReminderDto, ImportRemindersValidationOptions> _validator;

    public ImportRemindersStrategy(
        IUserRepository userRepository,
        IExcelManager excelManager,
        IImportValidator<ImportReminderDto, ImportRemindersValidationOptions> validator)
    {
        _excelManager = excelManager;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var reminders = _excelManager.Read<ImportReminderDto>(stream)
            .Where(u => !u.UserId.IsEmpty());

        var failed = new List<ImportEntityBase>();

        var userIds = reminders.Select(r => r.UserId)
            .Distinct();

        var users = await _userRepository.GetByExpressionAsync(u => userIds.Contains(u.Id), cancellationToken);

        var options = new ImportRemindersValidationOptions
        {
            Users = [.. users]
        };

        _validator.Set(options);

        var map = users.ToDictionary(u => u.Id);

        foreach (var row in _validator.ValidateAndSetErrors(reminders))
        {
            if (!row.CanBeImported())
            {
                failed.Add(row);
                continue;
            }

            if (!map.TryGetValue(row.UserId, out var user))
            {
                row.Exception = "User not found"; // TODO: Localize
                failed.Add(row);
                continue;
            }

            user.AddReminder(row.Id, row.Text, TimeOnly.Parse(row.NotifyAt));
        }

        foreach (var user in map.Values)
        {
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        return new ImportResult<ImportEntityBase>
        {
            FailedRows = failed,
        };
    }
}
