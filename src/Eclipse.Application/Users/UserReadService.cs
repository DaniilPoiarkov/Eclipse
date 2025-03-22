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
        var skip = (request.Page - 1) * request.PageSize;

        var count = await _repository.CountAsync(specification, cancellationToken);

        var users = (await _repository.GetByExpressionAsync(specification, skip, request.PageSize, cancellationToken))
            .Select(u => u.ToSlimDto())
            .ToArray();

        var pages = PaginatedList<UserSlimDto>.GetPagesCount(count, request.PageSize);

        return new PaginatedList<UserSlimDto>(users, pages, count);
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
