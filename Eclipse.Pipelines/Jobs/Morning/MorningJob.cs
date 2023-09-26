using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJob : EclipseJobBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramUserRepository _userRepository;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    public MorningJob(
        ICacheService cacheService,
        ITelegramUserRepository userRepository,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var users = _userRepository.GetAll();

        var notifications = new List<Task<IResult>>(users.Count);

        foreach (var user in users)
        {
            var isEnabled = _cacheService.Get<bool>(new CacheKey($"notifications-enabled-{user.Id}"));

            if (!isEnabled)
            {
                continue;
            }

            var pipeline = _pipelineProvider.Get("/daily_morning");

            var messageContext = new MessageContext(user.Id, string.Empty, user);

            notifications.Add(pipeline.RunNext(messageContext, context.CancellationToken));

            var key = new PipelineKey(user.Id);

            _pipelineStore.Remove(key);
            _pipelineStore.Set(pipeline, key);
        }

        var results = await Task.WhenAll(notifications);

        await Task.WhenAll(results.Select(s => s.SendAsync(_botClient, context.CancellationToken)));
    }
}
