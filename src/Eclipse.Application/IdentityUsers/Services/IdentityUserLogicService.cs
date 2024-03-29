﻿using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.IdentityUsers.Services;

internal sealed class IdentityUserLogicService : IIdentityUserLogicService
{
    private readonly IdentityUserManager _userManager;

    public IdentityUserLogicService(IdentityUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IdentityUserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.SetGmt(currentUserTime);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
