using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Reminders;

internal sealed class ImportRemindersStrategy : IImportStrategy
{
    public ImportType Type => ImportType.Reminders;


    private readonly IUserRepository _userRepository;

    private readonly IExcelManager _excelManager;

    public ImportRemindersStrategy(IUserRepository userRepository, IExcelManager excelManager)
    {
        _excelManager = excelManager;
        _userRepository = userRepository;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var reminders = _excelManager.Read<ImportReminderDto>(stream)
            .Where(u => u.UserId != Guid.Empty)
            .GroupBy(item => item.UserId);

        var failed = new List<ImportEntityBase>();

        var userIds = reminders.Select(r => r.Key).Distinct();

        var users = await _userRepository.GetByExpressionAsync(u => userIds.Contains(u.Id), cancellationToken);

        foreach (var grouping in reminders)
        {
            var user = users.FirstOrDefault(u => u.Id == grouping.Key);

            if (user is null)
            {
                SetErrors(grouping, "User not found"); // TODO: Localize
                failed.AddRange(grouping);
                continue;
            }

            foreach (var item in grouping)
            {
                try
                {
                    user.AddReminder(item.Id, item.Text, TimeOnly.Parse(item.NotifyAt));
                }
                catch (Exception ex)
                {
                    SetErrors([item], ex.Message);
                }
            }

            try
            {
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                SetErrors(grouping, ex.Message);
            }
        }

        return new ImportResult<ImportEntityBase>
        {
            FailedRows = failed,
        };
    }

    private static void SetErrors(IEnumerable<ImportEntityBase> models, string error)
    {
        foreach (var entity in models)
        {
            entity.Exception = error;
        }
    }
}
