using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Extensions;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.Services;

internal sealed class UserReadService : IUserReadService
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    public UserReadService(IUserRepository repository, ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        return [.. users.Select(u => u.ToDto())];
    }

    public async Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        var specification = request.Options.GetSpecification();
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

        await SetCreatedAtIfNullAsync(user, cancellationToken);

        return user.ToDto();
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        await SetCreatedAtIfNullAsync(user, cancellationToken);

        return user.ToDto();
    }

    private async Task SetCreatedAtIfNullAsync(User user, CancellationToken cancellationToken)
    {
        if (user.CreatedAt != default)
        {
            return;
        }

        user.SetCreatedAtIfNull(_timeProvider.Now);
        await _repository.UpdateAsync(user, cancellationToken);
    }
}
