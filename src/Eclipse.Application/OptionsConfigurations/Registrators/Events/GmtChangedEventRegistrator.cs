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

internal sealed class GmtChangedEventRegistrator : IApplicationServicesRegistrator
{
    public IServiceCollection Register(IServiceCollection services)
    {
        services.AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<GoodMorningJob>>()
            .AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<FinishTodoItemsJob>>()
            .AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<CollectMoodRecordJob>>()
            .AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<WeeklyMoodReportJob>>()
            .AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<MonthlyMoodReportJob>>()
            .AddTransient<IEventHandler<GmtChangedDomainEvent>, NewTimeEventHandler<CollectFeedbackJob>>();

        return services;
    }
}
