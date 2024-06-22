using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting.Models;
using Eclipse.Common.Excel;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Exporting;

internal sealed class ExportService : IExportService
{
    private readonly IExcelManager _excelManager;

    private readonly IUserRepository _userRepository;
    public ExportService(IExcelManager excelManager, IUserRepository userRepository)
    {
        _excelManager = excelManager;
        _userRepository = userRepository;
    }

    public async Task<MemoryStream> GetRemindersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var reminders = users.SelectMany(u => u.Reminders)
            .Select(r => new ExportReminderDto(r.Id, r.UserId, r.Text, r.NotifyAt));

        return _excelManager.Write(reminders);
    }

    public async Task<MemoryStream> GetTodoItemsAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var todoItems = users.SelectMany(u => u.TodoItems)
            .Select(t => new ExportTodoItemDto(t.Id, t.UserId, t.Text, t.IsFinished, t.CreatedAt, t.FinishedAt));

        return _excelManager.Write(todoItems);
    }

    public async Task<MemoryStream> GetUsersAsync(CancellationToken cancellationToken = default)
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
