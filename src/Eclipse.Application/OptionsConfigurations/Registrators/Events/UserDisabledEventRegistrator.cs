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

internal sealed class UserDisabledEventRegistrator : IApplicationServicesRegistrator
{
    public IServiceCollection Register(IServiceCollection services)
    {
        services.AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<GoodMorningJob>>()
            .AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<FinishTodoItemsJob>>()
            .AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<CollectMoodRecordJob>>()
            .AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<WeeklyMoodReportJob>>()
            .AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<MonthlyMoodReportJob>>()
            .AddTransient<IEventHandler<UserDisabledDomainEvent>, UserDisabledEventHandler<CollectFeedbackJob>>();

        return services;
    }
}
