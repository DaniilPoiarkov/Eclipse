using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Infrastructure.Cache;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJob : EclipseJobBase
{
    private readonly ICacheService _cacheService;

    private readonly IIdentityUserService _userService;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    public MorningJob(
        ICacheService cacheService,
        IIdentityUserService userService,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient)
    {
        _cacheService = cacheService;
        _userService = userService;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var users = (await _userService.GetAllAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled)
            .ToList();

        var notifications = new List<Task<IResult>>(users.Count);

        foreach (var user in users)
        {
            var isEnabled = _cacheService.Get<bool>(new CacheKey($"notifications-enabled-{user.Id}"));

            if (!isEnabled)
            {
                continue;
            }

            var pipeline = _pipelineProvider.Get("/daily_morning");

            var messageContext = new MessageContext(user.ChatId, string.Empty, new TelegramUser(user.ChatId, user.Name, user.Surname, user.Username));

            notifications.Add(pipeline.RunNext(messageContext, context.CancellationToken));

            var key = new PipelineKey(user.ChatId);

            _pipelineStore.Remove(key);
            _pipelineStore.Set(pipeline, key);
        }

        var results = await Task.WhenAll(notifications);

        await Task.WhenAll(results.Select(s => s.SendAsync(_botClient, context.CancellationToken)));
    }
}
