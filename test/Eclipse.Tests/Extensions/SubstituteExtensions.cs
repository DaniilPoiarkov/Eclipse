using Eclipse.Tests.CallHandlers;

using NSubstitute.Core;

namespace Eclipse.Tests.Extensions;

public static class SubstituteExtensions
{
    public static void DelegateCalls<TSubstitute, TInstance>(this TSubstitute substitute, TInstance instance)
        where TSubstitute : class
        where TInstance : TSubstitute
    {
        SubstitutionContext.Current.GetCallRouterFor(substitute)
            .RegisterCustomCallHandlerFactory(state => new RedirectAllCallsHandler<TInstance>(instance));
    }
}
