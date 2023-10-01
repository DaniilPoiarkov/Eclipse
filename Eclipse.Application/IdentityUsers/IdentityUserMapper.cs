using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

public class IdentityUserMapper : IMapper<IdentityUser, IdentityUserDto>
{
    public IdentityUserDto Map(IdentityUser value)
    {
        return new IdentityUserDto
        {
            Id = value.Id,
            Name = value.Name,
            Surname = value.Surname,
            Username = value.Username,
            ChatId = value.ChatId,
            Culture = value.Culture,
            NotificationsEnabled = value.NotificationsEnabled,
        };
    }
}
