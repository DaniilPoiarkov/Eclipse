using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Users;

internal sealed class ImportUsersStrategy : IImportStrategy
{
    public ImportType Type => ImportType.Users;

    private readonly UserManager _userManager;

    private readonly IExcelManager _excelManager;

    public ImportUsersStrategy(UserManager userManager, IExcelManager excelManager)
    {
        _userManager = userManager;
        _excelManager = excelManager;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var entities = _excelManager.Read<ImportUserDto>(stream)
            .Where(u => u.ChatId != default && u.Id != Guid.Empty);

        var failed = new List<ImportEntityBase>();

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

        return new ImportResult<ImportEntityBase>
        {
            FailedRows = failed
        };
    }
}
