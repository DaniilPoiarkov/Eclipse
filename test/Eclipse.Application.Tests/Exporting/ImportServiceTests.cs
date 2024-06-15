using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Exporting;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.Users;
using Eclipse.Infrastructure.Excel;
using Eclipse.Tests.Generators;

using FluentAssertions;

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

        await _userRepository.ReceivedWithAnyArgs().CreateAsync(default!);
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

        await _userRepository.DidNotReceiveWithAnyArgs().CreateAsync(default!);
    }

    [Fact]
    public async Task AddTodoItems_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.valid-todo-items.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        var user = UserGenerator.Generate(1).First();
        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await Sut.AddTodoItemsAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendMessageRequest>(x => x.Text == "All todo items imported successfully." && x.ChatId == _options.Value.Chat)
            );

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.TodoItems.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddTodoItems_WhenUserNotExist_ThenReportSend()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.valid-todo-items.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await Sut.AddTodoItemsAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following todo items")
            );

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    [Fact]
    public async Task AddReminders_WhenFileIsValid_ThenProcessedSeccessfully()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.valid-reminders.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        var user = UserGenerator.Generate(1).First();
        _userRepository.FindAsync(default).ReturnsForAnyArgs(user);

        await Sut.AddRemindersAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendMessageRequest>(x => x.Text == "All reminders imported successfully." && x.ChatId == _options.Value.Chat)
            );

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.Received().UpdateAsync(user);
        user.Reminders.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public async Task AddReminders_WhenUserNotExist_ThenReportSend()
    {
        using var stream = GetType().Assembly.GetManifestResourceStream("Eclipse.Application.Tests.Exporting.Files.valid-reminders.xlsx")
            ?? throw new InvalidOperationException("File not found.");

        using var ms = new MemoryStream();

        await stream.CopyToAsync(ms);

        _userRepository.FindAsync(default).ReturnsForAnyArgs(Task.FromResult<User?>(null));

        await Sut.AddRemindersAsync(ms);

        await _telegramBotClient.Received()
            .MakeRequestAsync(
                Arg.Is<SendDocumentRequest>(x => x.ChatId == _options.Value.Chat && x.Caption == "Failed to import following reminders")
            );

        await _userRepository.ReceivedWithAnyArgs().FindAsync(default);
        await _userRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
