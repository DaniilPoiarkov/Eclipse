﻿using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting.Users;

internal sealed class ImportUsersStrategy : IImportStrategy
{
    public ImportType Type => ImportType.Users;

    private readonly UserManager _userManager;

    private readonly IUserRepository _userRepository;

    private readonly IExcelManager _excelManager;

    private readonly IImportValidator<ImportUserDto, ImportUsersValidationOptions> _validator;

    public ImportUsersStrategy(
        UserManager userManager,
        IUserRepository userRepository,
        IExcelManager excelManager,
        IImportValidator<ImportUserDto, ImportUsersValidationOptions> validator)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _excelManager = excelManager;
        _validator = validator;
    }

    public async Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var rows = _excelManager.Read<ImportUserDto>(stream)
            .Where(u => u.ChatId != default && !u.Id.IsEmpty());

        var userIds = rows.Select(r => r.Id).Distinct();
        var userNames = rows.Select(r => r.UserName).Distinct();
        var chatIds = rows.Select(r => r.ChatId).Distinct();

        var users = await _userRepository.GetByExpressionAsync(
            u => userIds.Contains(u.Id)
            || userNames.Contains(u.UserName)
            || chatIds.Contains(u.ChatId),
            cancellationToken
        );

        var failed = new List<ImportEntityBase>();

        var options = new ImportUsersValidationOptions
        {
            Users = [.. users]
        };

        _validator.Set(options);

        foreach (var row in _validator.ValidateAndSetErrors(rows))
        {
            if (!row.CanBeImported())
            {
                failed.Add(row);
                continue;
            }

            try
            {
                var request = new CreateUserRequest
                {
                    Id = row.Id,
                    Name = row.Name,
                    Surname = row.Surname,
                    UserName = row.UserName,
                    ChatId = row.ChatId,
                    NotificationsEnabled = row.NotificationsEnabled,
                    Gmt = TimeSpan.Parse(row.Gmt),
                    IsEnabled = row.IsEnabled,
                    NewRegistered = false,
                };

                await _userManager.CreateAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                row.Exception = ex.Message;
                failed.Add(row);
            }
        }

        return new ImportResult<ImportEntityBase>
        {
            FailedRows = failed
        };
    }
}
