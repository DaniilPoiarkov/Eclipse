using Eclipse.Common.Session;
using Eclipse.Domain.Shared.Identity;

using System.Security.Claims;

namespace Eclipse.WebAPI.Session;

public sealed class CurrentSession : ICurrentSession
{
    public Guid? UserId { get; private set; }
    public long? ChatId { get; private set; }

    internal void Initialize(ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            UserId = userId;
        }

        var chatIdClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == CustomClaimTypes.ChatId);

        if (chatIdClaim is not null && long.TryParse(chatIdClaim.Value, out var chatId))
        {
            ChatId = chatId;
        }
    }
}
