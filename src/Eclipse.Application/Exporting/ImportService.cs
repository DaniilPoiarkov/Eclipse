using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting;

internal sealed class ImportService : IImportService
{
    private readonly UserManager _userManager;

    private readonly IExcelManager _excelManager;

    public ImportService(UserManager userManager, IExcelManager excelManager)
    {
        _userManager = userManager;
        _excelManager = excelManager;
    }

    public async Task<ImportResult<ImportReminderDto>> AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var reminders = _excelManager.Read<ImportReminderDto>(stream)
            .Where(u => u.UserId != Guid.Empty)
            .GroupBy(item => item.UserId);

        var failed = new List<ImportReminderDto>();

        foreach (var grouping in reminders)
        {
            var result = await _userManager.ImportRemindersAsync(grouping.Key, grouping, cancellationToken);

            if (result.IsSuccess)
            {
                continue;
            }

            SetErrors(grouping, result.Error.Description);

            failed.AddRange(grouping);
        }

        return new ImportResult<ImportReminderDto>
        {
            FailedRows = failed,
        };
    }

    public async Task<ImportResult<ImportTodoItemDto>> AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var todoItems = _excelManager.Read<ImportTodoItemDto>(stream)
            .Where(u => u.UserId != Guid.Empty)
            .GroupBy(item => item.UserId);

        var failed = new List<ImportTodoItemDto>();

        foreach (var grouping in todoItems)
        {
            var result = await _userManager.ImportTodoItemsAsync(grouping.Key, grouping, cancellationToken);

            if (result.IsSuccess)
            {
                continue;
            }

            SetErrors(grouping, result.Error.Description);

            failed.AddRange(grouping);
        }

        return new ImportResult<ImportTodoItemDto>
        {
            FailedRows = failed,
        };
    }

    public async Task<ImportResult<ImportUserDto>> AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var entities = _excelManager.Read<ImportUserDto>(stream)
            .Where(u => u.ChatId != default && u.Id != Guid.Empty);

        var failed = new List<ImportUserDto>();

        foreach (var entity in entities)
        {
            if (!entity.CanBeImported())
            {
                failed.Add(entity);
                continue;
            }

            try
            {
                await _userManager.ImportAsync(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                entity.Exception = ex.Message;
                failed.Add(entity);
            }
        }

        return new ImportResult<ImportUserDto>
        {
            FailedRows = failed
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
