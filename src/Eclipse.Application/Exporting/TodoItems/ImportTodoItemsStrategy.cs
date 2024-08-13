using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.TodoItems;

internal sealed class ImportTodoItemsStrategy : IImportStrategy
{
    public ImportType Type => ImportType.TodoItems;

    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;

    private readonly IImportValidator<ImportTodoItemDto, ImportTodoItemsValidationOptions> _validator;

    public ImportTodoItemsStrategy(IUserRepository userRepository, IExcelManager excelManager, IImportValidator<ImportTodoItemDto, ImportTodoItemsValidationOptions> validator)
    {
        _excelManager = excelManager;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var todoItems = _excelManager.Read<ImportTodoItemDto>(stream)
            .Where(u => u.UserId != Guid.Empty);

        var failed = new List<ImportEntityBase>();

        var userIds = todoItems.Select(i => i.UserId)
            .Distinct();

        var users = await _userRepository.GetByExpressionAsync(u => userIds.Contains(u.Id), cancellationToken);

        var map = users.ToDictionary(u => u.Id);

        var options = new ImportTodoItemsValidationOptions
        {
            Users = [.. users]
        };

        _validator.Set(options);

        foreach (var row in _validator.ValidateAndSetErrors(todoItems))
        {
            if (!row.CanBeImported())
            {
                failed.Add(row);
                continue;
            }

            if (!map.TryGetValue(row.UserId, out var user))
            {
                row.Exception = "User not found"; // TODO: localize
                failed.Add(row);
                continue;
            }

            var result = user.AddTodoItem(row.Id, row.Text, row.CreatedAt, row.IsFinished, row.FinishedAt);

            if (!result.IsSuccess)
            {
                row.Exception = result.Error.Description;
                failed.Add(row);
            }
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
