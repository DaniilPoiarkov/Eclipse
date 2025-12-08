using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Specifications;
using Eclipse.Domain.Feedbacks;
using Eclipse.Domain.Feedbacks.Specifications;

namespace Eclipse.Application.Feedbacks;

internal static class GetFeedbacksOptionsExtensions
{
    internal static Specification<Feedback> ToSpecification(this GetFeedbacksOptions options)
    {
        return Specification<Feedback>.Empty
            .AndIf(!options.UserIds.IsNullOrEmpty(), new PostedByUsersSpecification(options.UserIds))
            .AndIf(options.From.HasValue, new FromSpecification(options.From.GetValueOrDefault()))
            .AndIf(options.To.HasValue, new ToSpecification(options.To.GetValueOrDefault()));
    }
}
