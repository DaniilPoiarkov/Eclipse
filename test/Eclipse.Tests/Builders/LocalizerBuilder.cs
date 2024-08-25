using Microsoft.Extensions.Localization;

using NSubstitute;
using NSubstitute.Extensions;

namespace Eclipse.Tests.Builders;

public class LocalizerBuilder<T> : ILocalizerIndexerSubstituteBuilder<T>, ILocalizerBuilder<T>
{
    private readonly IStringLocalizer<T> _localizer;

    private string? _name;

    private object[]? _args;

    private LocalizerBuilder(IStringLocalizer<T> localizer)
    {
        _localizer = localizer;
    }

    public static ILocalizerBuilder<T> Create() => new LocalizerBuilder<T>(Substitute.For<IStringLocalizer<T>>());
    public static ILocalizerBuilder<T> Configure(IStringLocalizer<T> stringLocalizer) => new LocalizerBuilder<T>(stringLocalizer);

    public IStringLocalizer<T> Build() => _localizer;

    public ILocalizerIndexerSubstituteBuilder<T> ForWithArgs(string name, params object[] args)
    {
        _name = name;
        _args = args;

        return this;
    }

    public ILocalizerIndexerSubstituteBuilder<T> For(string name)
    {
        _name = name;
        return this;
    }

    public ILocalizerBuilder<T> Return(string value)
    {
        ArgumentNullException.ThrowIfNull(_name, nameof(_name));

        if (_args is null)
        {
            _localizer.Configure()
                [_name]
                .Returns(new LocalizedString(_name, value));
        }
        else
        {
            _localizer.Configure()
                [_name, _args]
                .Returns(new LocalizedString(_name, value));
        }

        return this;
    }
}
