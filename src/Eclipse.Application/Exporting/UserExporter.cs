using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting;

internal sealed class UserExporter : IUserExporter
{
    private readonly IUserRepository _userRepository;

    private readonly IExcelManager _excelManager;

    public UserExporter(IUserRepository userRepository, IExcelManager excelManager)
    {
        _userRepository = userRepository;
        _excelManager = excelManager;
    }

    public async Task<MemoryStream> ExportAllAsync(CancellationToken cancellationToken = default)
    {
        var users = (await _userRepository.GetAllAsync(cancellationToken))
            .Select(u => new ExportUserDto(
                u.Id,
                u.Name,
                u.Surname,
                u.UserName,
                u.ChatId,
                u.Culture,
                u.NotificationsEnabled,
                u.Gmt
            ));

        return _excelManager.Write(users);
    }
}
