using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Common.Specifications;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.Application.Feedbacks;

internal static class GetFeedbacksOptionsExtensions
{
    internal static Specification<Feedback> ToSpecification(this GetFeedbacksOptions options)
    {
        return Specification<Feedback>.Empty
            .AndIf(!options.UserIds.IsNullOrEmpty(), new CustomSpecification<Feedback>(f => options.UserIds.Contains(f.UserId)))
            .AndIf(options.From.HasValue, new CustomSpecification<Feedback>(f => f.CreatedAt >= options.From.GetValueOrDefault()))
            .AndIf(options.To.HasValue, new CustomSpecification<Feedback>(f => f.CreatedAt <= options.To.GetValueOrDefault()));
    }
}
