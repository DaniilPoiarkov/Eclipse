using Telegram.Bot.Types;

namespace Eclipse.Core.Routing;

[AttributeUsage(AttributeTargets.Class)]
public abstract class MyChatMemberPipelineAttribute : Attribute
{
    /// <summary>
    /// Determines whether this instance can handle the specified update.
    /// Requires to contain unqique logic to determine if the pipeline can handle the update, otherwise can lead to unexpected behavior due to retrieving the first pipeline which returns true.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns>
    ///   <c>true</c> if this instance can handle the specified update; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool CanHandle(Update update)
    {
        var chatMember = update.MyChatMember;

        if (chatMember is null)
        {
            return false;
        }

        return CanHandle(chatMember);
    }

    /// <summary>
    /// Determines whether this instance can handle the specified update.
    /// Requires to contain unqique logic to determine if the pipeline can handle the update, otherwise can lead to unexpected behavior due to retrieving the first pipeline which returns true.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns>
    ///   <c>true</c> if this instance can handle the specified update; otherwise, <c>false</c>.
    /// </returns>
    protected abstract bool CanHandle(ChatMemberUpdated chatMember);
}
