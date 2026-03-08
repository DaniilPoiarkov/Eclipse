using Telegram.Bot.Types;

namespace Eclipse.Core.Updates;

/// <summary>
/// Allows access current <a cref="Telegram.Bot.Types.Update"></a>
/// </summary>
public interface IUpdateAccessor
{
    /// <summary>
    /// Gets the update.
    /// </summary>
    Update? Update { get; }

    /// <summary>
    /// Sets the specified update.
    /// </summary>
    /// <param name="update">The update.</param>
    void Set(Update update);
}
