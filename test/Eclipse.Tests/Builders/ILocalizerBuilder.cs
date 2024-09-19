using Microsoft.Extensions.Localization;

namespace Eclipse.Tests.Builders;

public interface ILocalizerBuilder<T>
{
    ILocalizerIndexerSubstituteBuilder<T> ForWithArgs(string name, params object[] args);

    ILocalizerIndexerSubstituteBuilder<T> For(string name);

    IStringLocalizer<T> Build();
}
