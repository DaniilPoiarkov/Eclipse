namespace Eclipse.Core.Keywords;

internal sealed class NullKeywordMapper : IKeywordMapper
{
    public string Map(string keyword)
    {
        return keyword;
    }
}
