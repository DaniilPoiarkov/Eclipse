namespace Eclipse.Domain.Shared.ApiTokens;

public static class ApiTokensConsts
{
    public const int MaxCount = 5;

    public static readonly TimeSpan DefaultExpiration = TimeSpan.FromDays(90);
}
