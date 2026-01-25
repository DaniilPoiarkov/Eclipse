using Eclipse.Application;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting.Reminders;
using Eclipse.Common.Excel;
using Eclipse.Infrastructure.Excel;
using Eclipse.WebAPI.Background;

using Microsoft.Extensions.Options;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;

using Xunit;

namespace Eclipse.Tests.Background;

public sealed class ImportRemindersBackgroundJobTests
{
    private readonly IImportService _importService;

    private readonly IExcelManager _excelManager = new ExcelManager();

    private readonly ITelegramBotClient _botClient;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly Lazy<ImportRemindersBackgroundJob> _sut;

    private ImportRemindersBackgroundJob Sut => _sut.Value;

    public ImportRemindersBackgroundJobTests()
    {
        _importService = Substitute.For<IImportService>();
        _botClient = Substitute.For<ITelegramBotClient>();
        _options = Options.Create(new ApplicationOptions { Chat = 1 });
        _sut = new(() => new ImportRemindersBackgroundJob(_importService, _excelManager, _botClient, _options));
    }

    [Fact]
    public async Task WhenImportedSuccessfully_ThenSuccessNotificationSend()
    {
        var result = new ImportResult<ImportEntityBase>();

        _importService.AddRemindersAsync(Arg.Any<MemoryStream>(), TestContext.Current.CancellationToken)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecuteAsync(
            new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = TestsAssembly.ToBase64String("test") },
            TestContext.Current.CancellationToken
        );

        await _botClient.Received()
            .SendRequest(
                Arg.Is<SendMessageRequest>(x => x.Text == "All reminders imported successfully." && x.ChatId == _options.Value.Chat),
                TestContext.Current.CancellationToken
            );

        await _importService.ReceivedWithAnyArgs().AddRemindersAsync(Arg.Any<MemoryStream>(), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task WhenFailedResultReturned_ThenErrorNotificationSend()
    {
        var result = new ImportResult<ImportEntityBase>()
        {
            FailedRows = [
                new ImportReminderDto() { Exception = "test" },
                new ImportReminderDto() { Exception = "test" }
            ]
        };

        _importService.AddRemindersAsync(Arg.Any<MemoryStream>(), TestContext.Current.CancellationToken)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecuteAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = TestsAssembly.ToBase64String("test") }, TestContext.Current.CancellationToken);

        await _botClient.Received()
            .SendRequest(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following reminders"),
                TestContext.Current.CancellationToken
            );

        await _importService.ReceivedWithAnyArgs()
            .AddRemindersAsync(Arg.Any<MemoryStream>(), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task WhenBytesAreEmpty_ThenNoDependenciesCalled()
    {
        await Sut.ExecuteAsync(new ImportEntitiesBackgroundJobArgs(), TestContext.Current.CancellationToken);

        await _importService.DidNotReceiveWithAnyArgs()
            .AddRemindersAsync(Arg.Any<MemoryStream>(), TestContext.Current.CancellationToken);

        await _botClient.DidNotReceiveWithAnyArgs()
            .SendRequest(Arg.Any<IRequest<SendMessageRequest>>(), TestContext.Current.CancellationToken);
    }
}
