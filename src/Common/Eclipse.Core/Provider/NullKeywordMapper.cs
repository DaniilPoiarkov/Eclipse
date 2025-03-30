namespace Eclipse.Core.Provider;

internal sealed class NullKeywordMapper : IKeywordMapper
{
    public string Map(string keyword)
    {
        return keyword;
    }
}
