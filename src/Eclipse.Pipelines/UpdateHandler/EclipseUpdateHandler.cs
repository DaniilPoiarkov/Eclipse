using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Core.Context;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.UpdateParsing;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using System.Reflection;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.UpdateHandler;

internal sealed class EclipseUpdateHandler : IEclipseUpdateHandler
{
    public HandlerType Type => HandlerType.Active;

    private readonly ILogger<EclipseUpdateHandler> _logger;

    private readonly IUserService _userService;

    private readonly IPipelineStore _pipelineStore;

    private readonly IMessageStore _messageStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly IUpdateParser _updateParser;

    private readonly IStringLocalizer<EclipseUpdateHandler> _localizer;

    private static readonly UpdateType[] _allowedUpdateTypes =
    [
        UpdateType.Message,
        UpdateType.CallbackQuery,
        UpdateType.MyChatMember
    ];

    public EclipseUpdateHandler(
        ILogger<EclipseUpdateHandler> logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IUserService userService,
        IUpdateParser updateParser,
        IMessageStore messageStore,
        IStringLocalizer<EclipseUpdateHandler> localizer)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _userService = userService;
        _updateParser = updateParser;
        _messageStore = messageStore;
        _localizer = localizer;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!_allowedUpdateTypes.Contains(update.Type))
        {
            _logger.LogWarning("Update of type {updateType} is not supported", update.Type);
            return;
        }

        var context = _updateParser.Parse(update);

        if (context is null)
        {
            _logger.LogWarning("Context is null after parsing update of type {updateType}", update.Type);
            return;
        }

        var result = await HandleAndGetResultAsync(botClient, update, context, cancellationToken);

        if (result is RedirectResult redirect)
        {
            var command = redirect.PipelineType.GetCustomAttribute<RouteAttribute>()?.Command ?? string.Empty;

            var redirectUpdate = new Update
            {
                Message = new Message
                {
                    Text = command,
                    From = update.Message?.From,
                }
            };

            await HandleAndGetResultAsync(botClient, redirectUpdate, context, cancellationToken);
        }

        await AddOrUpdateAsync(context.User, cancellationToken);
    }

    private async Task<IResult> HandleAndGetResultAsync(ITelegramBotClient botClient, Update update, MessageContext context, CancellationToken cancellationToken)
    {
        var key = new PipelineKey(context.ChatId);

        var pipeline = await GetEclipsePipelineAsync(update, key);

        await _pipelineStore.RemoveAsync(key, cancellationToken);

        pipeline.SetLocalizer(_localizer);

        var result = await pipeline.RunNext(context, cancellationToken);

        var message = await result.SendAsync(botClient, cancellationToken);

        if (message is not null)
        {
            await _messageStore.SetAsync(new MessageKey(context.ChatId), message, cancellationToken);
        }

        if (!pipeline.IsFinished)
        {
            await _pipelineStore.SetAsync(key, pipeline, cancellationToken);
        }

        return result;
    }

    private async Task<Result<UserDto>> AddOrUpdateAsync(TelegramUser user, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByChatIdAsync(user.Id, cancellationToken);

        if (result.IsSuccess)
        {
            return await CheckAndUpdate(result.Value, user, cancellationToken);
        }

        var create = new UserCreateDto
        {
            Name = user.Name,
            UserName = user.UserName ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id,
            NotificationsEnabled = true,
        };

        return await _userService.CreateAsync(create, cancellationToken);
    }

    private async Task<Result<UserDto>> CheckAndUpdate(UserDto user, TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        if (HaveSameValues(user, telegramUser))
        {
            return user;
        }

        var update = new UserPartialUpdateDto
        {
            NameChanged = true,
            Name = telegramUser.Name,

            UserNameChanged = true,
            UserName = telegramUser.UserName,

            SurnameChanged = true,
            Surname = telegramUser.Surname
        };

        return await _userService.UpdatePartialAsync(user.Id, update, cancellationToken);

        static bool HaveSameValues(UserDto user, TelegramUser telegramUser)
        {
            return user.Name == telegramUser.Name
                && user.UserName == telegramUser.UserName
                && user.Surname == telegramUser.Surname;
        }
    }

    private async Task<EclipsePipelineBase> GetEclipsePipelineAsync(Update update, PipelineKey key)
    {
        return await GetPipelineAsync(update, key) as EclipsePipelineBase
            ?? new EclipseNotFoundPipeline();
    }

    private async Task<PipelineBase> GetPipelineAsync(Update update, PipelineKey key)
    {
        var pipeline = await _pipelineStore.GetOrDefaultAsync(key);

        if (pipeline is not null)
        {
            return pipeline;
        }

        return _pipelineProvider.Get(update);
    }
}
