using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Users;

internal sealed class ImportUsersStrategy : IImportStrategy
{
    public ImportType Type => ImportType.Users;

    private readonly UserManager _userManager;

    private readonly IExcelManager _excelManager;

    private readonly IImportValidator<ImportUserDto, ImportUsersValidationOptions> _validator;

    public ImportUsersStrategy(
        UserManager userManager,
        IExcelManager excelManager,
        IImportValidator<ImportUserDto, ImportUsersValidationOptions> validator)
    {
        _userManager = userManager;
        _excelManager = excelManager;
        _validator = validator;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var rows = _excelManager.Read<ImportUserDto>(stream)
            .Where(u => u.ChatId != default && !u.Id.IsEmpty());

        var userIds = rows.Select(r => new { r.Id, r.ChatId, r.UserName }).Distinct();

        var users = await _userManager.GetByExpressionAsync(
            u => userIds.Any(i => i.Id == u.Id
                || i.ChatId == u.ChatId
                || (!i.UserName.IsNullOrEmpty() && i.UserName == u.UserName)),
            cancellationToken
        );

        var failed = new List<ImportEntityBase>();
        
        var options = new ImportUsersValidationOptions
        {
            Users = [.. users]
        };

        _validator.Set(options);

        foreach (var entity in _validator.ValidateAndSetErrors(rows))
        {
            if (!entity.CanBeImported())
            {
                failed.Add(entity);
                continue;
            }

            try
            {
                var request = new CreateUserRequest
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Surname = entity.Surname,
                    UserName = entity.UserName,
                    ChatId = entity.ChatId,
                    NotificationsEnabled = entity.NotificationsEnabled,
                    Gmt = TimeSpan.Parse(entity.Gmt),
                    NewRegistered = false
                };

                await _userManager.CreateAsync(request, cancellationToken);
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
