using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.Users;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}
