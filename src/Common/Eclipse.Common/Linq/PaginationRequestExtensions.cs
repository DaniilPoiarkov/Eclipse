namespace Eclipse.Common.Linq;

public static class PaginationRequestExtensions
{
    public static int GetSkipCount<TOptions>(this PaginationRequest<TOptions> request)
        where TOptions : class, new()
    {
        return (request.Page - 1) * request.PageSize;
    }
}
