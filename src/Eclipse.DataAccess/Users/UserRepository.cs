using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.Users;

internal sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(EclipseDbContext context)
        : base(context) { }

    public Task<User?> FindByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(u => u.ChatId == chatId, cancellationToken);
    }

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (userName.IsNullOrEmpty())
        {
            return Task.FromResult<User?>(null);
        }

        return DbSet.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
