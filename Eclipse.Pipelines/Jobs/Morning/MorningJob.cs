using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Domain.IdentityUsers;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJob : EclipseJobBase
{
    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IIdentityUserRepository _identityUserRepository;

    public MorningJob(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IIdentityUserRepository identityUserRepository)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _identityUserRepository = identityUserRepository;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var users = await _identityUserRepository.GetByExpressionAsync(u => u.NotificationsEnabled, context.CancellationToken);
        var notifications = new List<Task<IResult>>(users.Count);

        foreach (var user in users)
        {
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
