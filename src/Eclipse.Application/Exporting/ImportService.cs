using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Import;

using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.Exporting;

internal sealed class ImportService : IImportService
{
    private readonly UserManager _userManager;

    private readonly IExcelManager _excelManager;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IOptions<TelegramOptions> _options;

    public ImportService(UserManager userManager, IExcelManager excelManager, ITelegramBotClient telegramBotClient, IOptions<TelegramOptions> options)
    {
        _userManager = userManager;
        _excelManager = excelManager;
        _telegramBotClient = telegramBotClient;
        _options = options;
    }

    public async Task AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var reminders = _excelManager.Read<ImportReminderDto>(stream)
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

        using var failedStream = _excelManager.Write(failed);

        await SendFailedExcel(
            failedStream,
            "failed-to-import-reminders.xlsx",
            "Failed to import following reminders",
            cancellationToken
        );
    }

    public async Task AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var todoItems = _excelManager.Read<ImportTodoItemDto>(stream)
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

        using var failedStream = _excelManager.Write(failed);

        await SendFailedExcel(
            failedStream,
            "failed-to-import-todo-items.xlsx",
            "Failed to import following todo items",
            cancellationToken
        );
    }

    public async Task AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var entities = _excelManager.Read<ImportUserDto>(stream);

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

        using var failedToImportUsersExcel = _excelManager.Write(failed);

        await SendFailedExcel(
            failedToImportUsersExcel,
            "failed-to-import-users.xlsx",
            "Failed to import following users",
            cancellationToken
        );
    }

    private static void SetErrors(IEnumerable<ImportEntityBase> models, string error)
    {
        foreach (var entity in models)
        {
            entity.Exception = error;
        }
    }

    private async Task SendFailedExcel(MemoryStream stream, string fileName, string caption, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendDocumentAsync(
            _options.Value.Chat,
            InputFile.FromStream(stream, fileName),
            caption: caption,
            cancellationToken: cancellationToken
        );
    }
}
