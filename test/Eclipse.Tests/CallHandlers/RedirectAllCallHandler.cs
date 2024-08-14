using NSubstitute.Core;

namespace Eclipse.Tests.CallHandlers;

internal sealed class RedirectAllCallHandler<T> : ICallHandler
{
    private readonly T _instance;

    public RedirectAllCallHandler(T instance)
    {
        _instance = instance;
    }

    public RouteAction Handle(ICall call)
    {
        return RouteAction.Return(call.GetMethodInfo().Invoke(_instance, call.GetArguments()));
    }
}
