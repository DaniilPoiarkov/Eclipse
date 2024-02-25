﻿namespace Eclipse.Application.Contracts.IdentityUsers;

[Serializable]
public sealed class GetUsersRequest
{
    public string? Name { get; set; }

    public string? UserName { get; set; }

    public bool NotificationsEnabled { get; set; }
}
