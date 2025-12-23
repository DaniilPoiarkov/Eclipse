using Eclipse.Application.Reminders.Sendings;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Pipelines.Reminders;

internal sealed class ReminderSender : IReminderSender
{
    private readonly IServiceProvider _serviceProvider;

    public ReminderSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default)
    {
        IReminderSenderStrategy strategy = arguments.RelatedItemId.HasValue
            ? _serviceProvider.GetRequiredService<HasRelatedItemReminderStrategy>()
            : _serviceProvider.GetRequiredService<SingleTimeReminderStrategy>();

        await strategy.Send(arguments, cancellationToken);
    }
}
