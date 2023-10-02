﻿using Eclipse.Core.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Builder;

public class CoreBuilder
{
    private readonly IServiceCollection _services;

    public CoreBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public CoreBuilder Decorate<TCoreDecorator>()
        where TCoreDecorator : class, ICoreDecorator
    {
        _services.AddScoped<ICoreDecorator, TCoreDecorator>();
        return this;
    }
}

public interface ICoreDecorator
{
    Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default);
}
