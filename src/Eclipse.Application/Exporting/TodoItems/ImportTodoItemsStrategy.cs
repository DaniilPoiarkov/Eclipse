using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.TodoItems;

internal sealed class ImportTodoItemsStrategy : IImportStrategy
{
    public ImportType Type => ImportType.TodoItems;

    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    public ImportTodoItemsStrategy(IUserRepository userRepository, IExcelManager excelManager)
    {
        _excelManager = excelManager;
        _userRepository = userRepository;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var todoItems = _excelManager.Read<ImportTodoItemDto>(stream)
            .Where(u => u.UserId != Guid.Empty)
            .GroupBy(item => item.UserId);

        var failed = new List<ImportEntityBase>();

        var userIds = todoItems.Select(i => i.Key)
            .Distinct();

        var users = await _userRepository.GetByExpressionAsync(u => userIds.Contains(u.Id), cancellationToken);

        foreach (var grouping in todoItems)
        {
            var user = users.FirstOrDefault(u => u.Id == grouping.Key);

            if (user is null)
            {
                SetErrors(grouping, "User not found"); // TODO: localize
                failed.AddRange(grouping);
                continue;
            }

            foreach (var record in grouping)
            {
                var result = user.AddTodoItem(record.Id, record.Text, record.CreatedAt, record.IsFinished, record.FinishedAt);

                if (!result.IsSuccess)
                {
                    record.Exception = result.Error.Description;
                    failed.Add(record);
                }
            }

            try
            {
                await _userRepository.UpdateAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                SetErrors(grouping, ex.Message);
                failed.AddRange(grouping);
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
