using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;

using Microsoft.Extensions.Options;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Exporting;

public sealed class ImportServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IExcelManager _excelManager = new ExcelManager();

    private readonly IOptions<TelegramOptions> _options;

    private readonly Lazy<IImportService> _sut;

    private IImportService Sut => _sut.Value;

    public ImportServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _telegramBotClient = Substitute.For<ITelegramBotClient>();

        _options = Options.Create(new TelegramOptions() { Chat = 1, Token = "" });

        _sut = new(() => new ImportService(
            new UserManager(_userRepository),
            _excelManager,
            _telegramBotClient,
            _options)
        );
    }

    [Fact]
    public async Task AddUsers_WhenFileIsValid_ThenProcessedSuccessfully()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.valid-users.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        await Sut.AddUsersAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendMessageRequest>(x => x.Text == "All users imported successfully." && x.ChatId == _options.Value.Chat)
            );
    }

    [Fact]
    public async Task AddUsers_WhenRecordsInvalid_ThenReportSend()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.invalid-users.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        await Sut.AddUsersAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following users")
            );
    }
}
