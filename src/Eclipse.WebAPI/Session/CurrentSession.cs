using Eclipse.Common.Session;
using Eclipse.Domain.Shared.Identity;

using System.Security.Claims;

namespace Eclipse.WebAPI.Session;

public sealed class CurrentSession : ICurrentSession
{
    private Guid? UserIdValue { get; set; }

    public Guid UserId
    {
        get
        {
            if (!UserIdValue.HasValue)
            {
                throw new InvalidOperationException($"Current session is not initialized or does not contain {nameof(UserId)}.");
            }

            return UserIdValue.Value;
        }
    }

    private long? ChatIdValue { get; set; }

    public long ChatId
    {
        get
        {
            if (!ChatIdValue.HasValue)
            {
                throw new InvalidOperationException($"Current session is not initialized or does not contain {nameof(ChatId)}.");
            }

            return ChatIdValue.Value;
        }
    }

    internal void Initialize(ClaimsPrincipal claimsPrincipal)
    {
        var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            UserIdValue = userId;
        }

        var chatIdClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == CustomClaimTypes.ChatId);

        if (chatIdClaim is not null && long.TryParse(chatIdClaim.Value, out var chatId))
        {
            ChatIdValue = chatId;
        }
    }
}
