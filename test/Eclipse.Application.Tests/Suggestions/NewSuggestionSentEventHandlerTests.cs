using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Users;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Options;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Suggestions;

public sealed class NewSuggestionSentEventHandlerTests
{
    private readonly ITelegramService _telegramService;

    private readonly IUserService _userService;

    private readonly IOptions<ApplicationOptions> _options;

    private readonly NewSuggestionSentEventHandler _sut;

    public NewSuggestionSentEventHandlerTests()
    {
        _telegramService = Substitute.For<ITelegramService>();
        _userService = Substitute.For<IUserService>();
        _options = Substitute.For<IOptions<ApplicationOptions>>();

        _sut = new NewSuggestionSentEventHandler(_telegramService, _options, _userService);
    }

    [Theory]
    [InlineData("test", 1, 2)]
    public async Task HandleAsync_WhenSenderExist_ThenIncludesUserDetailsInMessage(string text, long chatId, long adminChatId)
    {
        var user = UserGenerator.Get(chatId).ToDto();
        _userService.GetByChatIdAsync(chatId).Returns(user);

        var options = new ApplicationOptions { Chat = adminChatId };
        _options.Value.Returns(options);

        var @event = new NewSuggestionSentDomainEvent(Guid.CreateVersion7(), chatId, text);

        await _sut.Handle(@event);

        await _telegramService.Received().Send(
            Arg.Is<SendMessageModel>(m => m.ChatId == adminChatId
                && m.Message == $"Suggestion from {user.Name}{user.UserName.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}{@event.Text}"
            )
        );
    }

    [Theory]
    [InlineData("test", 1, 2)]
    public async Task HandleAsync_WhenSenderNotFound_ThenSendsDefaultMessage(string text, long chatId, long adminChatId)
    {
        _userService.GetByChatIdAsync(chatId).Returns(DefaultErrors.EntityNotFound<User>());

        var options = new ApplicationOptions { Chat = adminChatId };
        _options.Value.Returns(options);

        var @event = new NewSuggestionSentDomainEvent(Guid.CreateVersion7(), chatId, text);

        await _sut.Handle(@event);

        await _telegramService.Received().Send(
            Arg.Is<SendMessageModel>(m => m.ChatId == adminChatId
                && m.Message == $"Suggestion from unknown user with chat id {@event.ChatId}:{Environment.NewLine}{@event.Text}"
            )
        );
    }
}
