using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
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

    public Task AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        var users = _excelManager.Read<ImportUserDto>(stream);

        var failedToAddUsers = new List<ImportUserDto>();

        foreach (var user in users)
        {
            if (!user.CanBeImported())
            {
                failedToAddUsers.Add(user);
                continue;
            }

            try
            {
                await _userManager.ImportAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                user.Exception = ex.Message;
                failedToAddUsers.Add(user);
            }
        }

        if (failedToAddUsers.IsNullOrEmpty())
        {
            return;
        }

        using var failedToImportUsersExcel = _excelManager.Write(failedToAddUsers);

        await SendFailedExcel(
            failedToImportUsersExcel,
            "failed-to-upload-users.xlsx",
            "Failed to upload following users",
            cancellationToken
        );
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
