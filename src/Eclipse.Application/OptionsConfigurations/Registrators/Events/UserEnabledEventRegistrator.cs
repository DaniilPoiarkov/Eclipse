using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report.Monthly;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Application.OptionsConfigurations.Registrators.Events;

internal sealed class UserEnabledEventRegistrator : IApplicationServicesRegistrator
{
    public IServiceCollection Register(IServiceCollection services)
    {
        services.AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, GoodMorningJob>>()
            .AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, FinishTodoItemsJob>>()
            .AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, CollectMoodRecordJob>>()
            .AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, WeeklyMoodReportJob>>()
            .AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, MonthlyMoodReportJob>>()
            .AddTransient<IEventHandler<UserEnabledDomainEvent>, UserEventHandlerBase<UserEnabledDomainEvent, CollectFeedbackJob>>();

        return services;
    }
}
