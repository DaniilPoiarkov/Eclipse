using NSubstitute;

using Quartz;

namespace Eclipse.Tests.Fixtures;

/// <summary>
/// Simple configuration for <a cref="ISchedulerFactory">></a> to return <a cref="IScheduler"></a>
/// </summary>
public sealed class SchedulerFactoryFixture
{
    public readonly ISchedulerFactory SchedulerFactory;

    public readonly IScheduler Scheduler;

    public SchedulerFactoryFixture()
    {
        SchedulerFactory = Substitute.For<ISchedulerFactory>();
        Scheduler = Substitute.For<IScheduler>();

        SchedulerFactory.GetScheduler().Returns(Scheduler);
    }
}
