using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Specifications;
using Eclipse.Domain.Promotions;

namespace Eclipse.Application.Promotions;

internal static class GetPromotionsOptionsExtensions
{
    public static Specification<Promotion> ToSpecification(this GetPromotionsOptions options)
    {
        return Specification<Promotion>.Empty
            .AndIf(options.OnlyUnpublished, new CustomSpecification<Promotion>(p => p.TimesPublished == 0));
    }
}
