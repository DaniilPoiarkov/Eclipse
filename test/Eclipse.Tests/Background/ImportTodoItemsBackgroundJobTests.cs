﻿using Eclipse.Application.Contracts.Exporting;
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

public sealed class ImportTodoItemsBackgroundJobTests
{
    private readonly IImportService _importService;

    private readonly IExcelManager _excelManager = new ExcelManager();

    private readonly ITelegramBotClient _botClient;

    private readonly IOptions<TelegramOptions> _options;

    private readonly Lazy<ImportTodoItemsBackgroundJob> _sut;

    private ImportTodoItemsBackgroundJob Sut => _sut.Value;

    public ImportTodoItemsBackgroundJobTests()
    {
        _importService = Substitute.For<IImportService>();
        _botClient = Substitute.For<ITelegramBotClient>();
        _options = Options.Create(new TelegramOptions { Chat = 1, Token = "" });
        _sut = new(() => new ImportTodoItemsBackgroundJob(_importService, _excelManager, _botClient, _options));
    }

    [Fact]
    public async Task WhenImportedSuccessfully_ThenSuccessNotificationSend()
    {
        var result = new ImportResult<ImportTodoItemDto>();

        _importService.AddTodoItemsAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = Convert.ToBase64String([1, 2, 3]) });

        await _botClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendMessageRequest>(x => x.Text == "All todo items imported successfully." && x.ChatId == _options.Value.Chat)
            );

        await _importService.ReceivedWithAnyArgs().AddTodoItemsAsync(default!);
    }

    [Fact]
    public async Task WhenFailedResultReturned_ThenErrorNotificationSend()
    {
        var result = new ImportResult<ImportTodoItemDto>()
        {
            FailedRows = [
                new ImportTodoItemDto() { Exception = "test" },
                new ImportTodoItemDto() { Exception = "test" }
            ]
        };

        _importService.AddTodoItemsAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(result)
            );

        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs() { BytesAsBase64 = Convert.ToBase64String([1, 2, 3]) });

        await _botClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following todo items")
            );

        await _importService.ReceivedWithAnyArgs().AddTodoItemsAsync(default!);
    }

    [Fact]
    public async Task WhenBytesAreEmpty_ThenNoDependenciesCalled()
    {
        await Sut.ExecureAsync(new ImportEntitiesBackgroundJobArgs());

        await _importService.DidNotReceiveWithAnyArgs().AddTodoItemsAsync(default!);
        await _botClient.DidNotReceiveWithAnyArgs().MakeRequestAsync<SendMessageRequest>(default!);
    }
}
