using Eclipse.Core.Builder;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Updates;

namespace Eclipse.Pipelines.Users;

public sealed class UserTrackerDecoration : IPipelineExecutionDecorator
{
    private readonly IUserStore _userStore;

    private readonly IUpdateAccessor _updateAccessor;

    public UserTrackerDecoration(IUserStore userStore, IUpdateAccessor updateAccessor)
    {
        _userStore = userStore;
        _updateAccessor = updateAccessor;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var result = await execution(context, cancellationToken);

        if (_updateAccessor.Update is not null)
        {
            await _userStore.CreateOrUpdateAsync(context.User, _updateAccessor.Update, cancellationToken);
        }

        return result;
    }
}
