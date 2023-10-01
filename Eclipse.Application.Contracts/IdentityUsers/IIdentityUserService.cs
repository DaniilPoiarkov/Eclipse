using Eclipse.Domain.Shared.IdentityUsers;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserService
{
    Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}
