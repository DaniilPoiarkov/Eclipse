﻿using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal sealed class IdentityUserReadService : IIdentityUserReadService
{
    private readonly IMapper<IdentityUser, IdentityUserDto> _mapper;

    private readonly IdentityUserManager _userManager;

    private readonly IIdentityUserRepository _repository;

    public IdentityUserReadService(IMapper<IdentityUser, IdentityUserDto> mapper, IdentityUserManager userManager, IIdentityUserRepository repository)
    {
        _mapper = mapper;
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.GetAllAsync(cancellationToken);

        return users
            .Select(_mapper.Map)
            .ToList();
    }

    public async Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

        return _mapper.Map(user);
    }

    public async Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(IdentityUser));

        return _mapper.Map(user);
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetByFilterAsync(request.Name, request.UserName, request.NotificationsEnabled, cancellationToken);

        return users
            .Select(_mapper.Map)
            .ToArray();
    }
}
