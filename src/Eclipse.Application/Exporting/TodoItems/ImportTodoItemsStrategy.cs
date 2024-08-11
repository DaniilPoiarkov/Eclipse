using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.TodoItems;

internal sealed class ImportTodoItemsStrategy : IImportStrategy
{
    public ImportType Type => ImportType.TodoItems;

    private readonly UserManager _userManager;

    private readonly IExcelManager _excelManager;

    public ImportTodoItemsStrategy(UserManager userManager, IExcelManager excelManager)
    {
        _userManager = userManager;
        _excelManager = excelManager;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var todoItems = _excelManager.Read<ImportTodoItemDto>(stream)
            .Where(u => u.UserId != Guid.Empty)
            .GroupBy(item => item.UserId);

        var failed = new List<ImportEntityBase>();

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
