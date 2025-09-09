using Eclipse.Application.Jobs;

namespace Eclipse.Application.Notifications.FinishTodoItems;

internal record FinishTodoItemsSchedulerOptions(Guid UserId, TimeSpan Gmt) : ISchedulerOptions;
