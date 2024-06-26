using Eclipse.Application.Contracts.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Shared.Importing;
using Eclipse.Infrastructure.Excel;
using Eclipse.WebAPI.Background;

using Microsoft.Extensions.Options;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Tests.Background;

public sealed class ImportUsersBackgroundJobTests
{
    private readonly IImportService _importService;

    private readonly IExcelManager _excelManager = new ExcelManager();

    private readonly ITelegramBotClient _botClient;

    private readonly IOptions<TelegramOptions> _options;

    private readonly Lazy<ImportUsersBackgroundJob> _sut;

    private ImportUsersBackgroundJob Sut => _sut.Value;

    public ImportUsersBackgroundJobTests()
    {
        _importService = Substitute.For<IImportService>();
        _botClient = Substitute.For<ITelegramBotClient>();
        _options = Options.Create(new TelegramOptions { Chat = 1, Token = "" });
        _sut = new(() => new ImportUsersBackgroundJob(_importService, _excelManager, _botClient, _options));
    }

    [Fact]
    public async Task WhenImportedSuccessfully_ThenSuccessNotificationSend()
    {
        var result = new ImportResult<ImportUserDto>() { FailedRows = [] };

        _importService.AddUsersAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = Convert.ToBase64String([1, 2, 3]) });

        await _importService.ReceivedWithAnyArgs().AddUsersAsync(default!);

        await _botClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendMessageRequest>(x => x.Text == "All users imported successfully." && x.ChatId == _options.Value.Chat)
            );
    }

    [Fact]
    public async Task WhenFailedResultReturned_ThenErrorNotificationSend()
    {
        var result = new ImportResult<ImportUserDto>()
        {
            FailedRows = [
                new ImportUserDto() { Exception = "test" },
                new ImportUserDto() { Exception = "test" }
            ]
        };

        _importService.AddUsersAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = Convert.ToBase64String([1, 2, 3]) });

        await _botClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following users")
            );

        await _importService.ReceivedWithAnyArgs().AddUsersAsync(default!);
    }

    [Fact]
    public async Task WhenBytesAreEmpty_ThenNoDependenciesCalled()
    {
        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs());

        await _importService.DidNotReceiveWithAnyArgs().AddUsersAsync(default!);
        await _botClient.DidNotReceiveWithAnyArgs().MakeRequestAsync<SendMessageRequest>(default!);
    }
}
