using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Feedbacks;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.Jobs;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Domain.Feedbacks;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Feedbacks;
using Eclipse.Domain.Users;

using FluentAssertions;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Feedbacks;

public sealed class FeedbackServiceTests
{
    private readonly IFeedbackRepository _feedbackRepository;

    private readonly IUserRepository _userRepository;

    private readonly IBackgroundJobManager _backgroundJobManager;

    private readonly ITimeProvider _timeProvider;

    private readonly FeedbackService _sut;

    public FeedbackServiceTests()
    {
        _feedbackRepository = Substitute.For<IFeedbackRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
        _timeProvider = Substitute.For<ITimeProvider>();
        
        _sut = new FeedbackService(
            _feedbackRepository,
            _userRepository,
            _backgroundJobManager,
            _timeProvider
        );
    }

    [Fact]
    public async Task CreateAsync_WhenModelValid_ThenCreatesFeedback()
    {
        var userId = Guid.NewGuid();
        var model = new CreateFeedbackModel("Great app!", FeedbackRate.Excellent);

        var now = DateTime.UtcNow;
        _timeProvider.Now.Returns(now);
        
        var result = await _sut.CreateAsync(userId, model);

        await _feedbackRepository.Received(1).CreateAsync(
            Arg.Is<Feedback>(f => f.UserId == userId
                && f.Comment == model.Comment
                && f.Rate == model.Rate
                && f.CreatedAt == now
                && f.GetEvents().Any(e => e is FeedbackSentEvent)
            )
        );

        result.UserId.Should().Be(userId);
        result.Comment.Should().Be(model.Comment);
        result.Rate.Should().Be(model.Rate);
        result.CreatedAt.Should().Be(now);
    }

    [Fact]
    public async Task GetListAsync_WhenCalled_ThenReturnsPagedList()
    {
        var request = new PaginationRequest<GetFeedbacksOptions>
        {
            Options = new GetFeedbacksOptions(),
            Page = 1,
            PageSize = 2
        };

        var feedbacks = new List<Feedback>
        {
            Feedback.Create(Guid.NewGuid(), "Good app", FeedbackRate.Good, DateTime.UtcNow),
            Feedback.Create(Guid.NewGuid(), "Nice features", FeedbackRate.Excellent, DateTime.UtcNow)
        };

        _feedbackRepository.CountAsync(Arg.Any<Expression<Func<Feedback, bool>>>()).Returns(5);

        _feedbackRepository.GetByExpressionAsync(
            Arg.Any<Expression<Func<Feedback, bool>>>(),
            request.GetSkipCount(),
            request.PageSize
        ).Returns(feedbacks);

        var result = await _sut.GetListAsync(request);

        result.TotalCount.Should().Be(5);
        result.Items[0].Comment.Should().Be("Good app");
        result.Items[0].Rate.Should().Be(FeedbackRate.Good);
        result.Items[1].Comment.Should().Be("Nice features");
        result.Items[1].Rate.Should().Be(FeedbackRate.Excellent);
    }

    [Fact]
    public async Task RequestAsync_WhenRequest_ThenEnqueuesJobs()
    {
        var users = new List<User>
        {
            User.Create(Guid.CreateVersion7(), "user-1", "", "user1", 1, DateTime.UtcNow, true, false),
            User.Create(Guid.CreateVersion7(), "user-2", "", "user2", 2, DateTime.UtcNow, true, false),
        };

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(users);

        await _sut.RequestAsync();

        await _backgroundJobManager.Received(1).EnqueueAsync<RequestFeedbackBackgroundJob, UserIdJobData>(
            Arg.Is<UserIdJobData>(data => data.UserId == users[0].Id)
        );

        await _backgroundJobManager.Received(1).EnqueueAsync<RequestFeedbackBackgroundJob, UserIdJobData>(
            Arg.Is<UserIdJobData>(data => data.UserId == users[1].Id)
        );
    }
}
