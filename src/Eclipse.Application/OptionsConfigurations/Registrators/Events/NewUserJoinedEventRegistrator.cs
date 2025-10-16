using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Application.OptionsConfigurations.Registrators.Events;

internal sealed class NewUserJoinedEventRegistrator : IApplicationServicesRegistrator
{
    public IServiceCollection Register(IServiceCollection services)
    {
        services.AddTransient<IEventHandler<NewUserJoinedDomainEvent>, UserEventHandlerBase<NewUserJoinedDomainEvent, GoodMorningJob>>()
            .AddTransient<IEventHandler<NewUserJoinedDomainEvent>, UserEventHandlerBase<NewUserJoinedDomainEvent, FinishTodoItemsJob>>()
            .AddTransient<IEventHandler<NewUserJoinedDomainEvent>, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectMoodRecordJob>>()
            .AddTransient<IEventHandler<NewUserJoinedDomainEvent>, UserEventHandlerBase<NewUserJoinedDomainEvent, WeeklyMoodReportJob>>()
            .AddTransient<IEventHandler<NewUserJoinedDomainEvent>, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectFeedbackJob>>();

        return services;
    }
}
