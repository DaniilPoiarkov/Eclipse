using Microsoft.Extensions.Localization;

namespace Eclipse.Tests.Builders;

public interface ILocalizerBuilder<T>
{
    ILocalizerIndexerSubstituteBuilder<T> For(string name, params object[] args);

    IStringLocalizer<T> Build();
}
