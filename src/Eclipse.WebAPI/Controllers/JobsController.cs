using Microsoft.AspNetCore.Mvc;

using Quartz;
using Quartz.Impl.Matchers;

namespace Eclipse.WebAPI.Controllers;

[ApiController]
[Route("api/jobs")]
public sealed class JobsController : ControllerBase
{
    private readonly ISchedulerFactory _schedulerFactory;

    public JobsController(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs(CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);

        var jobs = new List<object>();

        foreach (var jobKey in jobKeys)
        {
            var detail = await scheduler.GetJobDetail(jobKey, cancellationToken);
            var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);
            jobs.Add(new
            {
                JobName = jobKey.Name,
                JobGroup = jobKey.Group,
                detail?.Description,
                Triggers = triggers.Select(trigger => new
                {
                    TriggerKey = trigger.Key.Name,
                    TriggerGroup = trigger.Key.Group,
                    NextFireTime = trigger.GetNextFireTimeUtc()?.DateTime,
                    PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.DateTime,
                    StartTime = trigger.StartTimeUtc.DateTime,
                    EndTime = trigger.EndTimeUtc?.DateTime,
                    TriggerType = trigger.GetType().Name
                })
            });
        }

        return Ok(jobs);
    }
}
