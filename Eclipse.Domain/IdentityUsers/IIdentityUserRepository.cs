﻿using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.IdentityUsers;

public interface IIdentityUserRepository : IRepository<IdentityUser>
{
    Task<IReadOnlyList<IdentityUser>> GetByFilterAsync(string? name, string? userName, bool notificationEnabled, CancellationToken cancellationToken = default);
}
