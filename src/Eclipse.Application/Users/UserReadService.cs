using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users;

internal sealed class UserReadService : IUserReadService
{
    private readonly IUserRepository _repository;

    public UserReadService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        return [.. users.Select(u => u.ToDto())];
    }

    public async Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        var specification = request.Options.ToSpecification();

        var count = await _repository.CountAsync(specification, cancellationToken);

        var users = await _repository.GetByExpressionAsync(
            specification,
            request.GetSkipCount(),
            request.PageSize,
            cancellationToken
        );
        
        var models = users.Select(u => u.ToSlimDto());

        return PaginatedList<UserSlimDto>.Create(models, count, request.PageSize);
    }

    public async Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return user.ToDto();
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return user.ToDto();
    }
}
