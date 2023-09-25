using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;

using Quartz;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJob : EclipseJobBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramUserRepository _userRepository;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    public MorningJob(
        ICacheService cacheService,
        ITelegramUserRepository userRepository,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var users = _userRepository.GetAll();

        var notifications = new List<Task>(users.Count);

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

        await Task.WhenAll(notifications);
    }
}
