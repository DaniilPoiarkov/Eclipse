﻿using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.IdentityUsers;

[Serializable]
public sealed class IdentityUserSlimDto : AggregateRootDto
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string Culture { get; set; } = string.Empty;

    public bool NotificationsEnabled { get; set; }
}
