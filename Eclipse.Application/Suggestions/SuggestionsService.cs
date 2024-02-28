using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Telegram;
using Eclipse.Common.Results;

using Microsoft.Extensions.Options;

namespace Eclipse.Application.Suggestions;

internal sealed class SuggestionsService : ISuggestionsService
{
    private readonly ISuggestionsSheetsService _sheetsService;

    private readonly IIdentityUserService _userService;

    private readonly ITelegramService _telegramService;

    private readonly IOptions<TelegramOptions> _options;

    public SuggestionsService(ISuggestionsSheetsService sheetsService, IIdentityUserService userService, ITelegramService telegramService, IOptions<TelegramOptions> options)
    {
        _sheetsService = sheetsService;
        _userService = userService;
        _telegramService = telegramService;
        _options = options;
    }

    public async Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        var suggestion = new SuggestionDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            TelegramUserId = request.TelegramUserId,
            Text = request.Text,
        };

        var getUserResult = await _userService.GetByChatIdAsync(request.TelegramUserId, cancellationToken);

        var message = getUserResult.IsSuccess
            ? $"Suggestion from {getUserResult.Value.Name}{getUserResult.Value.Username.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}{request.Text}"
            : $"Suggestion from unknown user:{Environment.NewLine}{request.Text}";

        var send = new SendMessageModel
        {
            ChatId = _options.Value.Chat,
            Message = message
        };

        await Task.WhenAll(
            _sheetsService.AddAsync(suggestion, cancellationToken),
            _telegramService.Send(send, cancellationToken));

        return Result.Success();
    }

    public async Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default)
    {
        var suggestions = _sheetsService.GetAll();
        var users = await _userService.GetAllAsync(cancellationToken);

        return suggestions.Join(users, s => s.TelegramUserId, u => u.ChatId, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user
        }).ToList();
    }
}
