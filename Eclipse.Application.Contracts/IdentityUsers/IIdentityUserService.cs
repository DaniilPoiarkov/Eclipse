using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserService : IIdentityUserReadService, IIdentityUserCreateUpdateService, IIdentityUserLogicService
{

}

public interface IIdentityUserReadService
{
    Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IdentityUserDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}

public interface IIdentityUserCreateUpdateService
{
    Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default);
}

public interface IIdentityUserLogicService
{
    Task<IdentityUserDto> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default);
}
