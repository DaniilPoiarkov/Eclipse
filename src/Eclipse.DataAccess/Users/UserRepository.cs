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
}
