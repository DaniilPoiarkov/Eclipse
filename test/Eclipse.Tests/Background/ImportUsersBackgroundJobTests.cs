﻿using Eclipse.Application;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting.Users;
using Eclipse.Common.Excel;
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

    private readonly IOptions<ApplicationOptions> _options;

    private readonly Lazy<ImportUsersBackgroundJob> _sut;

    private ImportUsersBackgroundJob Sut => _sut.Value;

    public ImportUsersBackgroundJobTests()
    {
        _importService = Substitute.For<IImportService>();
        _botClient = Substitute.For<ITelegramBotClient>();
        _options = Options.Create(new ApplicationOptions { Chat = 1 });
        _sut = new(() => new ImportUsersBackgroundJob(_importService, _excelManager, _botClient, _options));
    }

    [Fact]
    public async Task WhenImportedSuccessfully_ThenSuccessNotificationSend()
    {
        var result = new ImportResult<ImportEntityBase>() { FailedRows = [] };

        _importService.AddUsersAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecuteAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = TestsAssembly.ToBase64String("test") });

        await _importService.ReceivedWithAnyArgs().AddUsersAsync(default!);

        await _botClient.Received()
            .SendRequest(
                Arg.Is<SendMessageRequest>(x => x.Text == "All users imported successfully." && x.ChatId == _options.Value.Chat)
            );
    }

    [Fact]
    public async Task WhenFailedResultReturned_ThenErrorNotificationSend()
    {
        var result = new ImportResult<ImportEntityBase>()
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

        await Sut.ExecuteAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = TestsAssembly.ToBase64String("test") });

        await _botClient.Received()
            .SendRequest(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following users")
            );

        await _importService.ReceivedWithAnyArgs().AddUsersAsync(default!);
    }

    [Fact]
    public async Task WhenBytesAreEmpty_ThenNoDependenciesCalled()
    {
        await Sut.ExecuteAsync(new ImportEntitiesBackgroundJobArgs());

        await _importService.DidNotReceiveWithAnyArgs().AddUsersAsync(default!);
        await _botClient.DidNotReceiveWithAnyArgs().SendRequest<SendMessageRequest>(default!);
    }
}
