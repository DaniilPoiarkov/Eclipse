namespace Eclipse.Application.ApiTokens.Expiration;

public sealed class ApiTokenExpirationNotificationOptions
{
    public int[] Thresholds { get; set; } = [7, 3, 1];
}
